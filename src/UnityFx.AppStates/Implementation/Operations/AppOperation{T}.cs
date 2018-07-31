// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A yieldable asynchronous state operation.
	/// </summary>
	internal abstract class AppOperation<T> : AsyncResult<T>, ITraceable where T : class
	{
		#region data

		private const int _typeMask = 0x7;

		private readonly string _name;
		private readonly string _comment;
		private readonly AppStateService _stateManager;
		private readonly TraceSource _traceSource;

		#endregion

		#region interface

		protected AppStateService StateManager => _stateManager;
		protected IAppViewService ViewManager => _stateManager.ViewManager;
		protected IReadOnlyCollection<AppState> States => _stateManager.States;

		protected AppOperation(AppStateService stateManager, string name, string comment)
		{
			_name = name;
			_stateManager = stateManager;
			_traceSource = _stateManager.TraceSource;
			_comment = comment;
		}

		protected void TryDeactivateTopState()
		{
			_stateManager.TryDeactivateTopState(this);
		}

		protected void DismissAllStates()
		{
			_stateManager.DismissAllStates(this);
		}

		protected void DismissStateChildren(AppState state)
		{
			_stateManager.DismissStateChildren(this, state);
		}

		protected bool ProcessNonSuccess(IAsyncOperation op)
		{
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
			return AppViewController.GetId(controllerType) + " (" + args.ToString() + ')';
		}

		#endregion

		#region AsyncResult

		protected override void OnStatusChanged(AsyncOperationStatus status)
		{
			base.OnStatusChanged(status);

			if (status == AsyncOperationStatus.Running)
			{
				TraceStart();
			}
		}

		protected override void OnCompleted()
		{
			try
			{
				_stateManager.TryActivateTopState(this);
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
			_traceSource.TraceEvent(TraceEventType.Error, Id, _name + ": " + s);
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
			var s = _name;

			if (!string.IsNullOrEmpty(_comment))
			{
				s += ": " + _comment;
			}

			_traceSource.TraceEvent(TraceEventType.Start, Id, s);
		}

		private void TraceStop(AsyncOperationStatus status)
		{
			if (status == AsyncOperationStatus.RanToCompletion)
			{
				_traceSource.TraceEvent(TraceEventType.Stop, Id, _name + " completed");
			}
			else if (status == AsyncOperationStatus.Faulted)
			{
				_traceSource.TraceEvent(TraceEventType.Stop, Id, _name + " faulted");
			}
			else if (status == AsyncOperationStatus.Canceled)
			{
				_traceSource.TraceEvent(TraceEventType.Stop, Id, _name + " canceled");
			}
		}

		#endregion
	}
}
