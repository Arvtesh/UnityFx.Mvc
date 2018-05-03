// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;
using System.Threading;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	public class AppStateManagerTests : IDisposable
	{
		#region data

		private readonly AppStateService _stateManager;

		#endregion

		#region interface

		public AppStateManagerTests()
		{
			var viewFactory = Substitute.For<IAppViewManager>();
			var serviceProvider = Substitute.For<IServiceProvider>();
			_stateManager = new AppStateService(viewFactory, serviceProvider);
			_stateManager.Settings.DeeplinkScheme = "myapp";
			_stateManager.Settings.DeeplinkDomain = "game";
		}

		public void Dispose()
		{
			_stateManager.Dispose();
		}

		#endregion

		#region tests

		[Fact]
		public void InitialStateIsCorrect()
		{
			Assert.NotNull(_stateManager.States);
			Assert.Empty(_stateManager.States);
		}

		[Fact]
		public async Task PushStateAsync_Succeeds()
		{
			// Arrange
			var op = _stateManager.PresentAsync(typeof(TestController_Minimal), PresentArgs.Default);

			// Act
			var state = await op;

			// Assert
			Assert.NotNull(state);
			Assert.NotNull(state.Controller);
			Assert.True(state.IsActive);
			Assert.NotEmpty(_stateManager.States);
			Assert.Empty(state.Substates);
			Assert.IsType<TestController_Minimal>(state.Controller);
			Assert.Contains(state, _stateManager.States);
		}

		[Fact]
		public async Task PopStateAsync_Succeeds()
		{
			// Arrange
			var state = await _stateManager.PresentAsync(typeof(TestController_Minimal), PresentArgs.Default);

			// Act
			await _stateManager.PopStateAsync(state);

			// Assert
			Assert.NotNull(state);
			Assert.NotNull(state.Controller);
			Assert.False(state.IsActive);
			Assert.Empty(_stateManager.States);
		}

		[Fact]
		public void Dispose_CanBeCalledMultipleTimes()
		{
			_stateManager.Dispose();
			_stateManager.Dispose();
		}

		#endregion
	}
}
