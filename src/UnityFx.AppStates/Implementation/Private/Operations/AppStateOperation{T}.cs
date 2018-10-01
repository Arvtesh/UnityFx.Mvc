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
	internal abstract class AppStateOperation<T> : AsyncResult<T> where T : class
	{
		#region data

		private readonly AppStateService _stateManager;
		private readonly TraceSource _traceSource;

		#endregion

		#region interface

		protected AppStateService StateManager => _stateManager;
		protected IAppStateCollection States => _stateManager.States;

		protected AppStateOperation(AppStateService stateManager, object asyncState, string comment)
			: base(AsyncOperationStatus.Scheduled, asyncState)
		{
			Debug.Assert(stateManager != null);

			_stateManager = stateManager;
			_traceSource = _stateManager.TraceSource;

			if (string.IsNullOrEmpty(comment))
			{
				_stateManager.TraceEvent(TraceEventType.Verbose, ToString() + " initiated");
			}
			else
			{
				_stateManager.TraceEvent(TraceEventType.Verbose, ToString() + " initiated: " + comment);
			}
		}

		protected void TryActivateTopState()
		{
			// TODO: replace _stateManager.States.Count <= 1 check with something less hacky
			if (_stateManager.States.TryPeek(out var state) && !state.IsActive && _stateManager.States.Count <= 1)
			{
				(state as IPresentableEvents).OnActivate();
			}
		}

		protected void TryDeactivateTopState()
		{
			if (_stateManager.States.TryPeek(out var state) && state.IsActive)
			{
				(state as IPresentableEvents).OnDeactivate();
			}
		}

		protected void DismissAllStates()
		{
			while (_stateManager.States.TryPeek(out var state))
			{
				(state as IPresentableEvents).OnDismiss();
				state.Dispose();
			}
		}

		protected static string GetStateDesc(Type controllerType, PresentArgs args)
		{
			return Utility.GetControllerTypeId(controllerType) + " (" + args.ToString() + ')';
		}

		#endregion

		#region AsyncResult

		protected override void OnStarted()
		{
			TraceStart();
			TryDeactivateTopState();
		}

		protected override void OnCompleted()
		{
			try
			{
				TryActivateTopState();
			}
			finally
			{
				TraceStop(Status);
			}
		}

		protected override void OnCancel()
		{
			TrySetCanceled(false);
		}

		#endregion

		#region implementation

		private void TraceStart()
		{
			_stateManager.TraceEvent(TraceEventType.Start, ToString() + " started");
		}

		private void TraceStop(AsyncOperationStatus status)
		{
			if (status == AsyncOperationStatus.RanToCompletion)
			{
				_stateManager.TraceEvent(TraceEventType.Stop, ToString() + " completed");
			}
			else if (status == AsyncOperationStatus.Faulted)
			{
				_stateManager.TraceException(Exception);
				_stateManager.TraceEvent(TraceEventType.Stop, ToString() + " faulted");
			}
			else if (status == AsyncOperationStatus.Canceled)
			{
				_stateManager.TraceEvent(TraceEventType.Stop, ToString() + " canceled");
			}
		}

		#endregion
	}
}
