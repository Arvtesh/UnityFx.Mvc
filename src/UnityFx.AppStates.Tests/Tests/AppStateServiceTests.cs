// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;

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
		public void Present_CompletesSynchronouslyForMinimalViewController()
		{
			// Arrange/Act
			var op = _stateManager.PresentAsync(typeof(ViewController_Minimal));

			// Assert
			Assert.True(op.IsCompletedSuccessfully);
			Assert.True(op.CompletedSynchronously);
			Assert.IsType<ViewController_Minimal>(op.Result);
		}

		[Fact]
		public void Present_ThrowsForInvalidViewController()
		{
			// Arrange/Act/Assert
			Assert.Throws<ArgumentException>(() => _stateManager.PresentAsync(typeof(ViewController_Invalid)));
		}

		[Fact]
		public void Present_RaisesControllerEventsInCorrectOrder()
		{
			// Arrange/Act
			var op = _stateManager.PresentAsync<ViewController_Events>();
			var controller = op.Result;

			// Assert
			Assert.Equal(1, controller.PresentIndex);
			Assert.Equal(2, controller.OnPresentIndex);
			Assert.Equal(3, controller.OnActivateIndex);
			Assert.Equal(0, controller.OnDeactivateIndex);
			Assert.Equal(0, controller.OnDismissIndex);
			Assert.Equal(0, controller.DismissIndex);
			Assert.Equal(0, controller.DisposeIndex);
		}

		[Fact]
		public void Dismiss_RaisesControllerEventsInCorrectOrder()
		{
			// Arrange/Act
			var op = _stateManager.PresentAsync<ViewController_Events>();
			var controller = op.Result;
			controller.DismissAsync();

			// Assert
			Assert.Equal(1, controller.PresentIndex);
			Assert.Equal(2, controller.OnPresentIndex);
			Assert.Equal(3, controller.OnActivateIndex);
			Assert.Equal(4, controller.OnDeactivateIndex);
			Assert.Equal(5, controller.OnDismissIndex);
			Assert.Equal(6, controller.DismissIndex);
			Assert.Equal(7, controller.DisposeIndex);
		}
	}
}
