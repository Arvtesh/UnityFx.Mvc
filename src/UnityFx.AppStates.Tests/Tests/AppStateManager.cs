// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using Xunit;
using NSubstitute;

namespace UnityFx.App.Tests
{
	/// <summary>
	/// 
	/// </summary>
	public class AppStateManager : IDisposable
	{
		#region helper types

		private class TestController : IAppStateController
		{
		}

		private class TestControllerEvents : IAppStateController, IAppStateEvents
		{
			private int _counter = 0;

			public void OnActivate(bool firstTime)
			{
				Assert.Equal(2, ++_counter);
			}

			public void OnDeactivate()
			{
				Assert.Equal(3, ++_counter);
			}

			public void OnPop()
			{
				Assert.Equal(4, ++_counter);
			}

			public void OnPush()
			{
				Assert.Equal(1, ++_counter);
			}
		}

		#endregion

		#region data

		private readonly IAppStateService _stateManager;

		#endregion

		#region interface

		public AppStateManager()
		{
			var viewFactory = Substitute.For<IAppViewFactory>();
			var serviceProvider = Substitute.For<IServiceProvider>();
			_stateManager = AppStates.CreateStateManager(viewFactory, serviceProvider);
		}

		public void Dispose()
		{
			_stateManager.Dispose();
		}

		#endregion

		#region construction/disposing tests

		[Fact]
		public void InitialStateIsCorrect()
		{
			Assert.NotNull(_stateManager);

			Assert.NotNull(_stateManager.States);
			Assert.Empty(_stateManager.States);

			Assert.NotNull(_stateManager.Settings);
			Assert.NotNull(_stateManager.Settings.TraceListeners);
			Assert.NotNull(_stateManager.Settings.TraceSwitch);
		}

		[Fact]
		public void DisposeCanBeCalledMultipleTimes()
		{
			_stateManager.Dispose();
			_stateManager.Dispose();
		}

		#endregion

		#region push/pop tests

		[Fact]
		public async void PushStateSucceeds()
		{
			var state = await _stateManager.PushStateAsync<TestController>(PushOptions.None, null);

			Assert.NotNull(state);
			Assert.Equal("Test", state.Name);
			Assert.Contains(state, _stateManager.States);
		}

		[Fact]
		public async void PushStateRaisesControllerEvents()
		{
			var state = await _stateManager.PushStateAsync<TestControllerEvents>(PushOptions.None, null);
			await state.CloseAsync();

			Assert.Empty(_stateManager.States);
		}

		#endregion
	}
}
