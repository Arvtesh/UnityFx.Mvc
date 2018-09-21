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
	internal abstract class AppStateOperation<T> : AsyncResult<T>, ITraceable where T : class
	{
		#region data

		private readonly AppStateService _stateManager;
		private readonly TraceSource _traceSource;

		#endregion

		#region interface

		protected AppStateService StateManager => _stateManager;
		protected IAppViewService ViewManager => _stateManager.ViewManager;
		protected IAppStateCollection States => _stateManager.States;

		protected AppStateOperation(AppStateService stateManager, object asyncState, string comment)
			: base(AsyncOperationStatus.Scheduled, asyncState)
		{
			Debug.Assert(stateManager != null);

			_stateManager = stateManager;
			_traceSource = _stateManager.TraceSource;

			if (string.IsNullOrEmpty(comment))
			{
				TraceEvent(TraceEventType.Verbose, ToString() + " initiated");
			}
			else
			{
				TraceEvent(TraceEventType.Verbose, ToString() + " initiated: " + comment);
			}
		}

		protected void InvokeOnPresent(IViewController controller)
		{
			Debug.Assert(controller != null);
			TraceEvent(TraceEventType.Verbose, "Present " + controller.Id);

			if (controller is IPresentableEvents pe)
			{
				pe.OnPresent();
			}
		}

		protected void InvokeOnActivate(IViewController controller)
		{
			Debug.Assert(controller != null);
			TraceEvent(TraceEventType.Verbose, "Activate " + controller.Id);

			if (controller is IPresentableEvents pe)
			{
				pe.OnActivate();
			}
		}

		protected void InvokeOnDeactivate(IViewController controller)
		{
			Debug.Assert(controller != null);
			TraceEvent(TraceEventType.Verbose, "Deactivate " + controller.Id);

			if (controller is IPresentableEvents pe)
			{
				pe.OnDeactivate();
			}
		}

		protected void InvokeOnDismiss(IViewController controller)
		{
			Debug.Assert(controller != null);
			TraceEvent(TraceEventType.Verbose, "Dismiss " + controller.Id);

			if (controller is IPresentableEvents pe)
			{
				pe.OnDismiss();
			}
		}

		protected void TryActivateTopState()
		{
			// TODO: replace _stateManager.States.Count <= 1 check with something less hacky
			if (_stateManager.States.TryPeek(out var state) && !state.IsActive && _stateManager.States.Count <= 1)
			{
				(state as AppState).SetActive(true);
				InvokeOnActivate(state.Controller);
			}
		}

		protected void TryDeactivateTopState()
		{
			if (_stateManager.States.TryPeek(out var state) && state.IsActive)
			{
				(state as AppState).SetActive(false);
				InvokeOnDeactivate(state.Controller);
			}
		}

		protected void DismissAllStates()
		{
			while (_stateManager.States.TryPeek(out var state))
			{
				InvokeOnDismiss(state.Controller);
				state.Dispose();
			}
		}

		protected void DismissStateChildren(IAppState state)
		{
			Debug.Assert(state != null);

			foreach (var s in state.GetChildren().Reverse())
			{
				if (s == state)
				{
					break;
				}
				else if (s.IsChildOf(state))
				{
					InvokeOnDismiss(s.Controller);
					s.Dispose();
				}
			}
		}

		protected bool ProcessNonSuccess(IAsyncOperation op)
		{
			Debug.Assert(op != null);

			if (op.IsFaulted)
			{
				TrySetException(op.Exception, false);
				return false;
			}
			else if (op.IsCanceled)
			{
				TrySetCanceled(false);
				return false;
			}

			return true;
		}

		protected void Fail(Exception e)
		{
			TraceException(e);
			TrySetException(e, false);
		}

		protected static string GetStateDesc(Type controllerType, PresentArgs args)
		{
			return Utility.GetControllerTypeId(controllerType) + " (" + args.ToString() + ')';
		}

		#endregion

		#region AsyncResult

		protected override void OnStarted()
		{
			base.OnStarted();

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
				base.OnCompleted();
			}
		}

		protected override void OnCancel()
		{
			TrySetCanceled(false);
		}

		#endregion

		#region ITraceable

		public void TraceError(string s)
		{
			_traceSource.TraceEvent(TraceEventType.Error, Id, ToString() + ": " + s);
		}

		public void TraceException(Exception e)
		{
			_traceSource.TraceData(TraceEventType.Error, Id, e);
		}

		public void TraceEvent(TraceEventType eventType, string s)
		{
			_traceSource.TraceEvent(eventType, Id, s);
		}

		public void TraceData(TraceEventType eventType, object data)
		{
			_traceSource.TraceData(eventType, Id, data);
		}

		#endregion

		#region implementation

		private void TraceStart()
		{
			TraceEvent(TraceEventType.Start, ToString() + " started");
		}

		private void TraceStop(AsyncOperationStatus status)
		{
			if (status == AsyncOperationStatus.RanToCompletion)
			{
				TraceEvent(TraceEventType.Stop, ToString() + " completed");
			}
			else if (status == AsyncOperationStatus.Faulted)
			{
				TraceEvent(TraceEventType.Stop, ToString() + " faulted");
			}
			else if (status == AsyncOperationStatus.Canceled)
			{
				TraceEvent(TraceEventType.Stop, ToString() + " canceled");
			}
		}

		#endregion
	}
}
