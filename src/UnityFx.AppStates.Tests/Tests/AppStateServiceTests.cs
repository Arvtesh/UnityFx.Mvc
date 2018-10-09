// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	public class AppStateServiceTests
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly IAppStateService _stateManager;

		public AppStateServiceTests()
		{
			_serviceProvider = new DefaultServiceProvider();
			_stateManager = new AppStateService(_serviceProvider, null);
		}

		[Fact]
		public void Present_CompletesForMinimalViewController()
		{
			// Arrange/Act
			var op = _stateManager.PresentAsync(typeof(ViewController_Minimal));

			// Assert
			Assert.True(op.IsCompletedSuccessfully);
			Assert.True(op.CompletedSynchronously);
			Assert.IsType<ViewController_Minimal>(op.Result);
		}

		[Fact]
		public async Task Present_CompletesForMinimalViewControllerAsync()
		{
			// Arrange/Act
			var op = _stateManager.PresentAsync(typeof(ViewController_MinimalAsync));
			await op;

			// Assert
			Assert.True(op.IsCompletedSuccessfully);
			Assert.False(op.CompletedSynchronously);
			Assert.IsType<ViewController_MinimalAsync>(op.Result);
		}

		[Fact]
		public void Present_ThrowsForInvalidViewController()
		{
			// Arrange/Act/Assert
			Assert.Throws<ArgumentException>(() => _stateManager.PresentAsync(typeof(ViewController_Invalid)));
		}

		[Theory]
		[InlineData(typeof(ViewController_Events))]
		[InlineData(typeof(ViewController_EventsAsync))]
		public async Task Present_RaisesControllerEventsInCorrectOrder(Type controllerType)
		{
			// Arrange/Act
			var op = _stateManager.PresentAsync(controllerType);
			await op;

			var controller = (ViewController_Events)op.Result;

			// Assert
			Assert.Equal(1, controller.PresentIndex);
			Assert.Equal(2, controller.OnPresentIndex);
			Assert.Equal(3, controller.OnActivateIndex);
			Assert.Equal(0, controller.OnDeactivateIndex);
			Assert.Equal(0, controller.OnDismissIndex);
			Assert.Equal(0, controller.DismissIndex);
			Assert.Equal(0, controller.DisposeIndex);
		}

		[Theory]
		[InlineData(typeof(ViewController_Minimal))]
		[InlineData(typeof(ViewController_MinimalAsync))]
		public async Task Present_RaisesServiceEventsInCorrectOrder(Type controllerType)
		{
			// Arrange
			var index = 0;
			var presentInitiatedIndex = 0;
			var presentCompletedIndex = 0;

			_stateManager.PresentInitiated += (s, e) =>
			{
				presentInitiatedIndex = ++index;
			};

			_stateManager.PresentCompleted += (s, e) =>
			{
				presentCompletedIndex = ++index;
			};

			// Act
			var op = _stateManager.PresentAsync(controllerType);
			await op;

			// Assert
			Assert.Equal(1, presentInitiatedIndex);
			Assert.Equal(2, presentCompletedIndex);
		}

		[Fact]
		public async Task Present_PushesNewState()
		{
			// Arrange/Act
			await _stateManager.PresentAsync<ViewController_MinimalAsync>();

			// Assert
			Assert.NotEmpty(_stateManager.States);
			Assert.NotNull(_stateManager.ActiveState);
			Assert.True(_stateManager.ActiveState.IsActive);
		}

		[Theory]
		[InlineData(typeof(ViewController_Events))]
		[InlineData(typeof(ViewController_EventsAsync))]
		public async Task Dismiss_RaisesControllerEventsInCorrectOrder(Type controllerType)
		{
			// Arrange/Act
			var op = _stateManager.PresentAsync(controllerType);
			await op;
			await op.Result.DismissAsync();

			var controller = (ViewController_Events)op.Result;

			// Assert
			Assert.Equal(1, controller.PresentIndex);
			Assert.Equal(2, controller.OnPresentIndex);
			Assert.Equal(3, controller.OnActivateIndex);
			Assert.Equal(4, controller.OnDeactivateIndex);
			Assert.Equal(5, controller.OnDismissIndex);
			Assert.Equal(6, controller.DismissIndex);
			Assert.Equal(7, controller.DisposeIndex);
		}

		[Theory]
		[InlineData(typeof(ViewController_Minimal))]
		[InlineData(typeof(ViewController_MinimalAsync))]
		public async Task Dismiss_RaisesServiceEventsInCorrectOrder(Type controllerType)
		{
			// Arrange
			var index = 0;
			var dismissInitiatedIndex = 0;
			var dismissCompletedIndex = 0;

			_stateManager.DismissInitiated += (s, e) =>
			{
				dismissInitiatedIndex = ++index;
			};

			_stateManager.DismissCompleted += (s, e) =>
			{
				dismissCompletedIndex = ++index;
			};

			// Act
			var op = _stateManager.PresentAsync(controllerType);
			await op;
			await op.Result.DismissAsync();

			// Assert
			Assert.Equal(1, dismissInitiatedIndex);
			Assert.Equal(2, dismissCompletedIndex);
		}
	}
}
