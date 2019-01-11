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

namespace UnityFx.Mvc
{
	public class AppStateServiceTests
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly IPresentService _mvcService;

		public AppStateServiceTests()
		{
			_serviceProvider = new FakeServiceProvider();
			_mvcService = new PresentService(_serviceProvider, null);
		}

		[Fact]
		public void Present_CompletesForMinimalViewController()
		{
			// Arrange/Act
			var op = _mvcService.PresentAsync(typeof(ViewController_Minimal));

			// Assert
			Assert.True(op.IsCompletedSuccessfully);
			Assert.True(op.CompletedSynchronously);
			Assert.IsType<ViewController_Minimal>(op.Result);
		}

		[Fact]
		public async Task Present_CompletesForMinimalViewControllerAsync()
		{
			// Arrange/Act
			var op = _mvcService.PresentAsync(typeof(ViewController_MinimalAsync));
			await op;

			// Assert
			Assert.True(op.IsCompletedSuccessfully);
			Assert.False(op.CompletedSynchronously);
			Assert.IsType<ViewController_MinimalAsync>(op.Result);
		}

		[Theory]
		[InlineData(typeof(ViewController_Events))]
		[InlineData(typeof(ViewController_EventsAsync))]
		public async Task Present_RaisesControllerEventsInCorrectOrder(Type controllerType)
		{
			// Arrange/Act
			var op = _mvcService.PresentAsync(controllerType);
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

			_mvcService.PresentInitiated += (s, e) =>
			{
				presentInitiatedIndex = ++index;
			};

			_mvcService.PresentCompleted += (s, e) =>
			{
				presentCompletedIndex = ++index;
			};

			// Act
			var op = _mvcService.PresentAsync(controllerType);
			await op;

			// Assert
			Assert.Equal(1, presentInitiatedIndex);
			Assert.Equal(2, presentCompletedIndex);
		}

		[Fact]
		public async Task Present_PushesNewState()
		{
			// Arrange/Act
			await _mvcService.PresentAsync<ViewController_MinimalAsync>();

			// Assert
			Assert.NotNull(_mvcService.ActiveController);
		}

		[Fact]
		public void Present_ThrowsForInvalidViewController()
		{
			// Arrange/Act/Assert
			Assert.Throws<ArgumentException>(() => _mvcService.PresentAsync(typeof(ViewController_Invalid)));
		}

		[Theory]
		[InlineData(typeof(ViewController_Errors), ViewController_Errors.ThrowSource.Ctor)]
		[InlineData(typeof(ViewController_ErrorsAsync), ViewController_Errors.ThrowSource.Ctor)]
		[InlineData(typeof(ViewController_Errors), ViewController_Errors.ThrowSource.OnPresent)]
		[InlineData(typeof(ViewController_ErrorsAsync), ViewController_Errors.ThrowSource.OnPresent)]
		[InlineData(typeof(ViewController_Errors), ViewController_Errors.ThrowSource.Present)]
		[InlineData(typeof(ViewController_ErrorsAsync), ViewController_Errors.ThrowSource.Present)]
		public async Task Present_FailsOperationOnException(Type controllerType, ViewController_Errors.ThrowSource throwSource)
		{
			// Arrange
			var op = _mvcService.PresentAsync(controllerType, new ViewController_Errors.MyPresentArgs(throwSource));
			var exceptionThrown = false;

			// Act
			try
			{
				await op;
			}
			catch
			{
				exceptionThrown = true;
			}

			// Assert
			Assert.True(exceptionThrown);
			Assert.True(op.IsFaulted);
			Assert.Null(_mvcService.ActiveController);
		}

		[Theory]
		[InlineData(typeof(ViewController_Errors), ViewController_Errors.ThrowSource.OnActivate)]
		[InlineData(typeof(ViewController_ErrorsAsync), ViewController_Errors.ThrowSource.OnActivate)]
		public async Task Present_IgnoresExceptionInOnActivate(Type controllerType, ViewController_Errors.ThrowSource throwSource)
		{
			// Arrange/Act
			var op = _mvcService.PresentAsync(controllerType, new ViewController_Errors.MyPresentArgs(throwSource));
			await op;

			// Assert
			Assert.True(op.IsCompletedSuccessfully);
			Assert.NotNull(_mvcService.ActiveController);
		}

		[Theory]
		[InlineData(typeof(ViewController_Minimal))]
		[InlineData(typeof(ViewController_MinimalAsync))]
		public async Task Present_IgnoresCompletionEventExceptions(Type controllerType)
		{
			// Arrange
			_mvcService.PresentCompleted += (s, e) =>
			{
				throw new Exception();
			};

			// Act/Assert
			await _mvcService.PresentAsync(controllerType);
		}

		[Theory]
		[InlineData(typeof(ViewController_Events))]
		[InlineData(typeof(ViewController_EventsAsync))]
		public async Task Dismiss_RaisesControllerEventsInCorrectOrder(Type controllerType)
		{
			// Arrange/Act
			var op = _mvcService.PresentAsync(controllerType);
			await op;
			//await op.Result.DismissAsync();

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

			_mvcService.DismissInitiated += (s, e) =>
			{
				dismissInitiatedIndex = ++index;
			};

			_mvcService.DismissCompleted += (s, e) =>
			{
				dismissCompletedIndex = ++index;
			};

			// Act
			var op = _mvcService.PresentAsync(controllerType);
			await op;
			//await op.Result.DismissAsync();

			// Assert
			Assert.Equal(1, dismissInitiatedIndex);
			Assert.Equal(2, dismissCompletedIndex);
		}

		[Theory]
		[InlineData(typeof(ViewController_Minimal))]
		[InlineData(typeof(ViewController_MinimalAsync))]
		public async Task Dismiss_IgnoresCompletionEventExceptions(Type controllerType)
		{
			// Arrange
			_mvcService.DismissCompleted += (s, e) =>
			{
				throw new Exception();
			};

			// Act/Assert
			var op = _mvcService.PresentAsync(controllerType);
			await op;
			//await op.Result.DismissAsync();
		}

		[Theory]
		[InlineData(typeof(ViewController_Errors), ViewController_Errors.ThrowSource.OnDeactivate)]
		[InlineData(typeof(ViewController_ErrorsAsync), ViewController_Errors.ThrowSource.OnDeactivate)]
		[InlineData(typeof(ViewController_Errors), ViewController_Errors.ThrowSource.OnDismiss)]
		[InlineData(typeof(ViewController_ErrorsAsync), ViewController_Errors.ThrowSource.OnDismiss)]
		[InlineData(typeof(ViewController_Errors), ViewController_Errors.ThrowSource.Dismiss)]
		[InlineData(typeof(ViewController_ErrorsAsync), ViewController_Errors.ThrowSource.Dismiss)]
		public async Task Dismiss_FailsOperationOnException(Type controllerType, ViewController_Errors.ThrowSource throwSource)
		{
			// Arrange
			var op = _mvcService.PresentAsync(controllerType, new ViewController_Errors.MyPresentArgs(throwSource));
			var exceptionThrown = false;

			// Act
			try
			{
				await op;
				//await op.Result.DismissAsync();
			}
			catch
			{
				exceptionThrown = true;
			}

			// Assert
			Assert.True(exceptionThrown);
			Assert.Null(_mvcService.ActiveController);
		}
	}
}
