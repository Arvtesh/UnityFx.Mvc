// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;
using System.Threading;

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
			var viewFactory = Substitute.For<IAppStateViewFactory>();
			var serviceProvider = Substitute.For<IServiceProvider>();
			_stateManager = new AppStateService(SynchronizationContext.Current, viewFactory, serviceProvider);
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
			Assert.NotNull(_stateManager);

			Assert.NotNull(_stateManager.States);
			Assert.Empty(_stateManager.States);

			Assert.NotNull(_stateManager.TraceListeners);
			Assert.NotNull(_stateManager.TraceSwitch);
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
		public async Task PushStateSucceeds()
		{
			var state = await _stateManager.PushStateTaskAsync(typeof(TestController_Minimal), PushStateArgs.Default);

			Assert.NotNull(state);
			Assert.NotNull(state.Controller);
			Assert.True(state.IsActive);
			Assert.Empty(state.ChildStates);
			Assert.IsType<TestController_Minimal>(state.Controller);
			Assert.Contains(state, _stateManager.States);
		}

		[Fact]
		public void DisposeCanBeCalledMultipleTimes()
		{
			_stateManager.Dispose();
			_stateManager.Dispose();
		}

		#endregion
	}
}
