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
	internal abstract class AppStateOperation : AsyncResult<IAppState>, IAppStateOperationInfo
	{
		#region data

		private const int _typeMask = 0x7;

		private readonly int _id;
		private readonly string _name;
		private readonly AppStateManager _stateManager;
		private readonly TraceSource _traceSource;

		private static int _lastId;

		private List<Exception> _exceptions;

		#endregion

		#region interface

		protected AppStateManager StateManager => _stateManager;

		protected AppStateOperation(AppStateManager stateManager, AppStateOperationType opType, AsyncCallback asyncCallback, object asyncState, string comment)
		{
			_id = (++_lastId << 3) | (int)opType;
			_name = $"{opType.ToString()} ({_id.ToString(CultureInfo.InvariantCulture)})";
			_stateManager = stateManager;
			_traceSource = _stateManager.TraceSource;

			TraceStart(comment);
		}

		protected void TraceError(string s)
		{
			_traceSource.TraceEvent(TraceEventType.Error, _id, _name + ": " + s);
		}

		protected void TraceException(Exception e)
		{
			_traceSource.TraceData(TraceEventType.Error, _id, e);
		}

		protected void TraceEvent(TraceEventType eventType, string s)
		{
			_traceSource.TraceEvent(eventType, _id, s);
		}

		protected void TraceData(TraceEventType eventType, object data)
		{
			_traceSource.TraceData(eventType, _id, data);
		}

		#endregion

		#region AsyncResult

		protected override void OnStatusChanged(AsyncOperationStatus status)
		{
			base.OnStatusChanged(status);
			TraceStop(status);
		}

		#endregion

		#region IAppStateOperationInfo

		public int OperationId => _id;
		public AppStateOperationType OperationType => (AppStateOperationType)(_id & _typeMask);
		public object UserState => AsyncState;

		#endregion

		#region IExceptionAggregator

		public void AddException(Exception e)
		{
			if (_exceptions == null)
			{
				_exceptions = new List<Exception>() { e };
			}
			else
			{
				_exceptions.Add(e);
			}
		}

		#endregion

		#region implementation

		private void TraceStart(string comment)
		{
			var s = _name;

			if (!string.IsNullOrEmpty(comment))
			{
				s += ": " + comment;
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
