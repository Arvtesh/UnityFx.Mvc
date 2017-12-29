// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;

namespace UnityFx.AppStates.Tests
{
	public enum ControllerMethodId
	{
		None,
		Ctor,
		OnPush,
		OnLoadContent,
		OnActivate,
		OnDectivate,
		OnPop,
		Dispose
	}

	public struct MethodCallInfo
	{
		public object Caller;
		public ControllerMethodId Method;

		public MethodCallInfo(object caller, ControllerMethodId method)
		{
			Caller = caller;
			Method = method;
		}
	}

	public class AppStateController : IDisposable
	{
		#region data

		private readonly IAppViewFactory _viewFactory;
		private readonly IServiceProvider _serviceProvider;
		private readonly IAppStateService _stateManager;

		#endregion

		#region interface

		public AppStateController()
		{
			_viewFactory = Substitute.For<IAppViewFactory>();
			_serviceProvider = Substitute.For<IServiceProvider>();
			_stateManager = AppStateFactory.CreateStateManager(_viewFactory, _serviceProvider);
		}

		public void Dispose()
		{
			_stateManager.Dispose();
		}

		#endregion

		#region tests

		[Fact]
		public void InvalidControllerTypeShouldThrow()
		{
			Assert.ThrowsAsync<ArgumentException>(() => _stateManager.PushStateAsync(typeof(TestController_Invalid), PushOptions.None, null));
		}

		[Fact]
		public async Task MultipleConstructorArgumentsSupported()
		{
			var testDependency = new object();
			_serviceProvider.GetService(typeof(object)).Returns(testDependency);

			var state = await _stateManager.PushStateAsync<TestController_ConstructorWithMultipleArguments>(PushOptions.None, null);
			var controller = state.Controller as TestController_ConstructorWithMultipleArguments;

			Assert.NotNull(state);
			Assert.NotNull(controller);
			Assert.Equal(_serviceProvider, controller.ServiceProvider);
			Assert.Equal<object>(state, controller.Context);
			Assert.Equal(testDependency, controller.Obj);
		}

		[Fact]
		public async Task EventsAreTriggeredInCorrectOrder()
		{
			var eventList = new List<MethodCallInfo>();
			var state = await _stateManager.PushStateAsync<TestController_Events>(PushOptions.None, eventList);
			await state.CloseAsync();

			Assert.Empty(_stateManager.States);
			Assert.Equal(ControllerMethodId.Ctor, eventList[0].Method);
			Assert.Equal(ControllerMethodId.OnPush, eventList[1].Method);
			Assert.Equal(ControllerMethodId.OnLoadContent, eventList[2].Method);
			Assert.Equal(ControllerMethodId.OnActivate, eventList[3].Method);
			Assert.Equal(ControllerMethodId.OnDectivate, eventList[4].Method);
			Assert.Equal(ControllerMethodId.OnPop, eventList[5].Method);
			Assert.Equal(ControllerMethodId.Dispose, eventList[6].Method);
		}

		[Theory]
		[InlineData(ControllerMethodId.OnPush, ControllerMethodId.Ctor)]
		[InlineData(ControllerMethodId.OnActivate, ControllerMethodId.OnActivate)]
		public async Task SubstateEventShouldComeAfter(ControllerMethodId stateEvent, ControllerMethodId substateEvent)
		{
			var eventList = new List<MethodCallInfo>();
			var state = await _stateManager.PushStateAsync<TestController_EventsSubstsatesCtor>(PushOptions.None, eventList);

			AssertBefore(stateEvent, state.Controller, substateEvent, eventList);
		}

		[Theory]
		[InlineData(ControllerMethodId.OnDectivate, ControllerMethodId.OnDectivate)]
		[InlineData(ControllerMethodId.OnPop, ControllerMethodId.Dispose)]
		public async Task SubstateEventShouldComeBefore(ControllerMethodId stateEvent, ControllerMethodId substateEvent)
		{
			var eventList = new List<MethodCallInfo>();
			var state = await _stateManager.PushStateAsync<TestController_EventsSubstsatesCtor>(PushOptions.None, eventList);
			var stateController = state.Controller;
			await state.CloseAsync();

			AssertAfter(stateEvent, stateController, substateEvent, eventList);
		}

		[Theory]
		[InlineData(ControllerMethodId.Ctor)]
		[InlineData(ControllerMethodId.OnPush)]
		[InlineData(ControllerMethodId.OnLoadContent)]
		[InlineData(ControllerMethodId.OnActivate)]
		public async Task PushExceptionIsForwarded(ControllerMethodId method)
		{
			await Assert.ThrowsAsync<Exception>(() => _stateManager.PushStateAsync<TestController_EventErrors>(PushOptions.None, method));

			if (method == ControllerMethodId.OnActivate)
			{
				Assert.Equal(1, _stateManager.States.Count);
			}
			else
			{
				Assert.Empty(_stateManager.States);
			}
		}

		[Theory]
		[InlineData(ControllerMethodId.OnDectivate)]
		[InlineData(ControllerMethodId.OnPop)]
		[InlineData(ControllerMethodId.Dispose)]
		public async Task PopExceptionIsForwarded(ControllerMethodId method)
		{
			var state = await _stateManager.PushStateAsync<TestController_EventErrors>(PushOptions.None, method);
			await Assert.ThrowsAsync<Exception>(() => state.CloseAsync());
			Assert.Empty(_stateManager.States);
		}

		#endregion

		#region implementation

		private static void AssertBefore(ControllerMethodId method, ControllerMethodId method2, List<MethodCallInfo> calls)
		{
			var index = calls.FindIndex(ci => ci.Method == method);
			var index2 = calls.FindIndex(ci => ci.Method == method2);
			Assert.NotEqual(-1, index);
			Assert.NotEqual(-1, index2);
			Assert.True(index < index2);
		}

		private static void AssertBefore(ControllerMethodId method, object caller, ControllerMethodId method2, List<MethodCallInfo> calls)
		{
			var index = calls.FindIndex(ci => ci.Caller == caller && ci.Method == method);
			var index2 = calls.FindIndex(ci => ci.Caller != caller && ci.Method == method2);
			Assert.NotEqual(-1, index);
			Assert.NotEqual(-1, index2);
			Assert.True(index < index2);
		}

		private static void AssertAfter(ControllerMethodId method, ControllerMethodId method2, List<MethodCallInfo> calls)
		{
			var index = calls.FindIndex(ci => ci.Method == method);
			var index2 = calls.FindIndex(ci => ci.Method == method2);
			Assert.NotEqual(-1, index);
			Assert.NotEqual(-1, index2);
			Assert.True(index > index2);
		}

		private static void AssertAfter(ControllerMethodId method, object caller, ControllerMethodId method2, List<MethodCallInfo> calls)
		{
			var index = calls.FindIndex(ci => ci.Caller == caller && ci.Method == method);
			var index2 = calls.FindIndex(ci => ci.Caller != caller && ci.Method == method2);
			Assert.NotEqual(-1, index);
			Assert.NotEqual(-1, index2);
			Assert.True(index > index2);
		}

		#endregion
	}
}
