// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;
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
			var viewFactory = new DummyViewService();
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

		private class PresentController1 : AppViewController
		{
			public PresentController1(IAppViewControllerContext c) : base(c) { }
		}

		[Fact]
		public async Task Present_tt()
		{
			// Arrange
			var op = _stateManager.PresentAsync<PresentController1>(PresentArgs.Default);

			// Act
			await op;

			// Assert
			Assert.True(op.IsCompletedSuccessfully);
			Assert.NotEmpty(_stateManager.States);
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
