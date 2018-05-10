// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	internal class AppStateTransitionManager : IAppStateTransitionManager
	{
		public IAsyncOperation PlayDismissTransition(AppView view)
		{
			return AsyncResult.CompletedOperation;
		}

		public IAsyncOperation PlayPresentTransition(AppView view)
		{
			return AsyncResult.CompletedOperation;
		}

		public IAsyncOperation PlayTransition(AppView fromView, AppView toView)
		{
			return AsyncResult.CompletedOperation;
		}
	}
}
