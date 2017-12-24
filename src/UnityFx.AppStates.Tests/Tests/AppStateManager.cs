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
	/// Common <see cref="IAppStateManager"/> tests.
	/// </summary>
	public class AppStateManager : IDisposable
	{
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

		#region tests

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
		public void AllMethodsThrowWhenDisposed()
		{
			_stateManager.Dispose();

			Assert.Throws<ObjectDisposedException>(() => _stateManager.GetStatesRecursive());
			Assert.Throws<ObjectDisposedException>(() => _stateManager.GetStatesRecursive(new List<IAppState>()));
			Assert.Throws<ObjectDisposedException>(() => _stateManager.Settings);
			Assert.Throws<ObjectDisposedException>(() => _stateManager.States);
			Assert.Throws<ObjectDisposedException>(() => _stateManager.PushStateAsync<TestController_Minimal>(PushOptions.None, null).Wait());
			Assert.Throws<ObjectDisposedException>(() => _stateManager.PushStateAsync(typeof(TestController_Minimal), PushOptions.None, null).Wait());
		}

		[Fact]
		public async Task PushStateSucceeds()
		{
			var state = await _stateManager.PushStateAsync<TestController_Minimal>(PushOptions.None, null);

			Assert.NotNull(state);
			Assert.NotNull(state.Controller);
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
