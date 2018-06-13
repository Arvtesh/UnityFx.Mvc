// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityFx.Async;
using Xunit;
using NSubstitute;

namespace UnityFx.AppStates
{
	public class AppStateControllerTests : IDisposable
	{
		#region data

		private readonly IAppViewService _viewFactory;
		private readonly IServiceProvider _serviceProvider;
		private readonly AppStateService _stateManager;

		#endregion

		#region interface

		public AppStateControllerTests()
		{
			_viewFactory = Substitute.For<IAppViewService>();
			_serviceProvider = Substitute.For<IServiceProvider>();
			_stateManager = new AppStateService(_viewFactory, _serviceProvider);
		}

		public void Dispose()
		{
			_stateManager.Dispose();
		}

		#endregion

		#region tests

		[Fact]
		public void InvalidControllerTypeShouldThrow()
		{
			Assert.ThrowsAsync<ArgumentException>(() => _stateManager.PresentAsync(typeof(TestController_Invalid), PresentArgs.Default).ToTask());
		}

		#endregion

		#region implementation

		#endregion
	}
}
