// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace UnityFx.App.Tests
{
	internal class TestController_LoadContentError : IAppStateController, IAppStateContent
	{
		public Task LoadContent(CancellationToken cancellationToken)
		{
			throw new Exception();
		}
	}
}
