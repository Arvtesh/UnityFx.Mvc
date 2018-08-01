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
		}

		public void Dispose()
		{
			_stateManager.Dispose();
		}

		#endregion

		#region smoke tests

		private class PresentController1 : AppViewController
		{
			public PresentController1(IPresentableContext c) : base(c) { }
		}

		[Fact]
		public async Task SmokeTest()
		{
			// Arrange
			var op = _stateManager.PresentAsync<PresentController1>(PresentArgs.Default);

			// Act
			await op;

			// Assert
			Assert.True(op.IsCompletedSuccessfully);
			Assert.Single(_stateManager.States);
		}

		#endregion

		#region tests

		[Fact]
		public void Dispose_CanBeCalledMultipleTimes()
		{
			_stateManager.Dispose();
			_stateManager.Dispose();
		}

		#endregion
	}
}
