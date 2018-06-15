// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	internal class DummyViewService : IAppViewService
	{
		public IReadOnlyCollection<AppView> Views => throw new NotImplementedException();

		public AppView CreateChildView(string id, AppView parent, AppViewOptions options)
		{
			return new DummyView(id, options);
		}

		public AppView CreateView(string id, AppView insertAfter, AppViewOptions options)
		{
			return new DummyView(id, options);
		}

		public void Dispose()
		{
		}

		public IAsyncOperation PlayDismissTransition(AppView view, AppView toView)
		{
			return AsyncResult.Delay(1);
		}

		public IAsyncOperation PlayPresentTransition(AppView fromView, AppView toView, bool replace)
		{
			return AsyncResult.Delay(1);
		}
	}
}
