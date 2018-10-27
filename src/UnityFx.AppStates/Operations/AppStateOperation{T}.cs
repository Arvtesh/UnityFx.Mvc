// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A yieldable asynchronous state operation.
	/// </summary>
	internal abstract class AppStateOperation<T> : AsyncResult<T>, IAppStateOperation where T : class
	{
		#region data

		private readonly AppStateService _stateManager;
		private bool _completedSynchronously = true;

		#endregion

		#region interface

		protected AppStateService StateManager => _stateManager;
		protected IAppStateCollection States => _stateManager.States;

		protected AppStateOperation(AppStateService stateManager, object asyncState)
			: base(AsyncOperationStatus.Scheduled, asyncState)
		{
			Debug.Assert(stateManager != null);

			_stateManager = stateManager;
		}

		protected void DismissAllStates()
		{
			while (_stateManager.States.TryPeek(out var state))
			{
				(state as IPresentableEvents).OnDismiss();
				state.Dispose();
			}
		}

		protected new bool TrySetCanceled()
		{
			return TrySetCanceled(_completedSynchronously);
		}

		protected new bool TrySetException(Exception e)
		{
			return TrySetException(e, _completedSynchronously);
		}

		protected new bool TrySetCompleted()
		{
			return TrySetCompleted(_completedSynchronously);
		}

		protected new bool TrySetResult(T result)
		{
			return TrySetResult(result, _completedSynchronously);
		}

		protected static string GetStateDesc(Type controllerType, PresentArgs args)
		{
			return Utility.GetControllerTypeId(controllerType) + " (" + args.ToString() + ')';
		}

		#endregion

		#region IAppStateOperation

		public void SetCompletedAsynchronously()
		{
			_completedSynchronously = false;
		}

		#endregion

		#region implementation

		#endregion
	}
}
