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
			var op = _stateManager.PresentAsync<ViewController_Minimal>();

			// Assert
			Assert.True(op.IsCompletedSuccessfully);
			Assert.True(op.CompletedSynchronously);
		}

		[Fact]
		public void Present_ThrowsForInvalidViewController()
		{
			// Arrange/Act/Assert
			Assert.Throws<ArgumentException>(() => _stateManager.PresentAsync(typeof(ViewController_Invalid)));
		}
	}
}
