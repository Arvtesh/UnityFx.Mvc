// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
//using NSubstitute;

namespace UnityFx.Mvc
{
	public class PresentServiceTests
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly IViewFactory _viewFactory;
		private readonly IPresentService _mvcService;

		public PresentServiceTests()
		{
			_serviceProvider = new FakeServiceProvider();
			_mvcService = new PresentService(_serviceProvider);
		}

		[Fact]
		public void Present_MinimalControllerCanBePresented()
		{
			var presentResult = _mvcService.Present<MinimalPresentable>();

			Assert.NotNull(presentResult);
			Assert.NotNull(presentResult.Controller);
			Assert.False(presentResult.IsDismissed);
			Assert.NotEmpty(_mvcService.Controllers);
			Assert.Contains(presentResult.Controller, _mvcService.Controllers);
		}

		[Fact]
		public async Task Present_MinimalControllerCanBePresentedAsync()
		{
			var presentResult = _mvcService.Present<MinimalPresentable>();
			var controller = await presentResult;

			Assert.NotNull(controller);
			Assert.True(presentResult.IsPresented);
			Assert.False(presentResult.IsDismissed);
			Assert.NotEmpty(_mvcService.Controllers);
			Assert.Contains(controller, _mvcService.Controllers);
		}

		[Fact]
		public void Present_InvokesLoadViewAsync()
		{
			var presentResult = _mvcService.Present<CallbackPresentable>();

			Assert.NotNull(presentResult);
			Assert.NotNull(presentResult.Controller);
		}

		[Fact]
		public async Task Present_InvokesOnActivate()
		{
			var controller = await _mvcService.Present<CallbackPresentable>();

			Assert.Equal(1, controller.OnActivateCounter);
		}

		[Fact]
		public void Dismiss_CanBeCalledBeforePresentCompleted()
		{
			var presentResult = _mvcService.Present<MinimalPresentable>();
			presentResult.Dismiss();

			Assert.True(presentResult.IsDismissed);
		}

		[Fact]
		public async Task Dismiss_InvokesOnDeactivate()
		{
			var controller = await _mvcService.Present<CallbackPresentable>();
			controller.Dispose();

			Assert.Equal(1, controller.OnDeactivateCounter);
		}
	}
}
