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
			var viewFactory = Substitute.For<IAppStateViewManager>();
			var serviceProvider = Substitute.For<IServiceProvider>();
			_stateManager = new AppStateService(SynchronizationContext.Current, viewFactory, serviceProvider);
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
		public void AllMethodsThrowWhenDisposed()
		{
			_stateManager.Dispose();

			Assert.Throws<ObjectDisposedException>(() => _stateManager.GetStatesRecursive());
			Assert.Throws<ObjectDisposedException>(() => _stateManager.GetStatesRecursive(new List<IAppState>()));
			Assert.Throws<ObjectDisposedException>(() => _stateManager.States);
			Assert.Throws<ObjectDisposedException>(() => _stateManager.PushStateTaskAsync(typeof(TestController_Minimal), PushStateArgs.Default).Wait());
		}

		[Fact]
		public async Task PushStateAsync_Succeeds()
		{
			// Arrange
			var op = _stateManager.PushStateAsync(typeof(TestController_Minimal), PushStateArgs.Default);

			// Act
			var state = await op;

			// Assert
			Assert.NotNull(state);
			Assert.NotNull(state.Controller);
			Assert.True(state.IsActive);
			Assert.NotEmpty(_stateManager.States);
			Assert.Empty(state.ChildStates);
			Assert.IsType<TestController_Minimal>(state.Controller);
			Assert.Contains(state, _stateManager.States);
		}

		[Fact]
		public async Task PopStateAsync_Succeeds()
		{
			// Arrange
			var state = await _stateManager.PushStateAsync(typeof(TestController_Minimal), PushStateArgs.Default);

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
