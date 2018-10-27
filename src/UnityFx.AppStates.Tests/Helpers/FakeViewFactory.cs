// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	internal class FakeViewFactory : IViewFactory
	{
		public IAsyncOperation<IView> LoadViewAsync(string name, string resourceId, ViewOptions options, IView insertAfter)
		{
			return AsyncResult.FromResult<IView>(new FakeView());
		}
	}
}
