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
	internal abstract class AppOperation : AsyncResult<AppViewController>
	{
		#region data

		private const int _typeMask = 0x7;

		private readonly int _id;
		private readonly string _name;
		private readonly string _comment;
		private readonly AppStateService _stateManager;
		private readonly TraceSource _traceSource;

		private static int _lastId;

		#endregion

		#region interface

		public int Id => _id;

		protected AppStateService StateManager => _stateManager;
		protected IAppViewService ViewManager => _stateManager.ViewManager;
		protected AppStateCollection States => _stateManager.States;

		protected AppOperation(AppStateService stateManager, string name, string comment)
		{
			_id = ++_lastId;
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

		public void TraceError(string s)
		{
			_traceSource.TraceEvent(TraceEventType.Error, _id, _name + ": " + s);
		}

		public void TraceException(Exception e)
		{
			_traceSource.TraceData(TraceEventType.Error, _id, e);
		}

		public void TraceEvent(TraceEventType eventType, string s)
		{
			_traceSource.TraceEvent(eventType, _id, s);
		}

		public void TraceData(TraceEventType eventType, object data)
		{
			_traceSource.TraceData(eventType, _id, data);
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

		#region implementation

		private void TraceStart()
		{
			var s = _name;

			if (!string.IsNullOrEmpty(_comment))
			{
				s += ": " + _comment;
			}

			_traceSource.TraceEvent(TraceEventType.Start, _id, s);
		}

		private void TraceStop(AsyncOperationStatus status)
		{
			if (status == AsyncOperationStatus.RanToCompletion)
			{
				_traceSource.TraceEvent(TraceEventType.Stop, _id, _name + " completed");
			}
			else if (status == AsyncOperationStatus.Faulted)
			{
				_traceSource.TraceEvent(TraceEventType.Stop, _id, _name + " faulted");
			}
			else if (status == AsyncOperationStatus.Canceled)
			{
				_traceSource.TraceEvent(TraceEventType.Stop, _id, _name + " canceled");
			}
		}

		#endregion
	}
}
