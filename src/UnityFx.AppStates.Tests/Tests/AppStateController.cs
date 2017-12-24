// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;
using System.Threading;

namespace UnityFx.App.Tests
{
	public enum ControllerMethodId
	{
		None,
		Ctor,
		OnPush,
		LoadContent,
		OnActivate,
		OnDectivate,
		OnPop,
		Dispose
	}

	/// <summary>
	/// Test realted to <see cref="IAppStateController"/>.
	/// </summary>
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
			_stateManager = AppStates.CreateStateManager(_viewFactory, _serviceProvider);
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
		public async Task AllEventsAreTriggered()
		{
			var state = await _stateManager.PushStateAsync<TestController_Events>(PushOptions.None, null);
			var controller = state.Controller as TestController_Events;
			await state.CloseAsync();

			Assert.Empty(_stateManager.States);
			Assert.Equal(1, controller.OnPushIndex);
			Assert.Equal(2, controller.LoadContentIndex);
			Assert.Equal(3, controller.OnActivateIndex);
			Assert.Equal(4, controller.OnDeactivateIndex);
			Assert.Equal(5, controller.OnPopIndex);
			Assert.Equal(6, controller.DisposeIndex);
		}

		[Theory]
		[InlineData(ControllerMethodId.Ctor)]
		[InlineData(ControllerMethodId.OnPush)]
		[InlineData(ControllerMethodId.LoadContent)]
		[InlineData(ControllerMethodId.OnActivate)]
		public async Task PushExceptionIsForwarded(ControllerMethodId method)
		{
			await Assert.ThrowsAsync<Exception>(() => _stateManager.PushStateAsync<TestController_EventErrors>(PushOptions.None, method));
			Assert.Empty(_stateManager.States);
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
	}
}
