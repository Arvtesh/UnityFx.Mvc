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
	/// <summary>
	/// 
	/// </summary>
	public class AppStateManager_Controller : IDisposable
	{
		#region data

		private readonly IAppStateService _stateManager;
		private readonly int _stateManagerThreadId;

		#endregion

		#region interface

		public AppStateManager_Controller()
		{
			var viewFactory = Substitute.For<IAppViewFactory>();
			var serviceProvider = Substitute.For<IServiceProvider>();
			_stateManager = AppStates.CreateStateManager(viewFactory, serviceProvider);
			_stateManagerThreadId = Thread.CurrentThread.ManagedThreadId;
		}

		public void Dispose()
		{
			_stateManager.Dispose();
		}

		#endregion

		#region tests

		[Fact]
		public async Task PushStateSucceeds()
		{
			var state = await _stateManager.PushStateAsync<TestController_Minimal>(PushOptions.None, null);

			Assert.NotNull(state);
			Assert.Contains(state, _stateManager.States);
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

		[Fact]
		public async Task EventHandlerExceptionsAreIgnored()
		{
			var state = await _stateManager.PushStateAsync<TestController_EventErrors>(PushOptions.None, null);
			await state.CloseAsync();
		}

		[Fact]
		public async Task DisposeExceptionIsForwarded()
		{
			var state = await _stateManager.PushStateAsync<TestController_DisposeError>(PushOptions.None, null);
			await Assert.ThrowsAsync<Exception>(() => state.CloseAsync());
		}

		[Fact]
		public async Task LoadContentExceptionIsForwarded()
		{
			await Assert.ThrowsAsync<Exception>(() => _stateManager.PushStateAsync<TestController_LoadContentError>(PushOptions.None, null));
			Assert.Empty(_stateManager.States);
		}

		[Fact]
		public async Task ConstructorExceptionIsForwarded()
		{
			await Assert.ThrowsAsync<Exception>(() => _stateManager.PushStateAsync<TestController_ConstructorError>(PushOptions.None, null));
			Assert.Empty(_stateManager.States);
		}

		#endregion
	}
}
