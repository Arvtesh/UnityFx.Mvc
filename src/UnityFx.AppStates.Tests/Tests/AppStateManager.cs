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
		private class TestController : IAppStateController
		{
		}

		private readonly IAppStateService _stateManager;

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

		[Fact]
		public async void PushStateSucceeds()
		{
			var state = await _stateManager.PushStateAsync<TestController>(PushOptions.None, null);

			Assert.NotNull(state);
			Assert.Contains(state, _stateManager.States);
		}
	}
}
