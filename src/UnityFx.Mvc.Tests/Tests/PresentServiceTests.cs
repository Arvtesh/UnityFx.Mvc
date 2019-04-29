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
	}
}
