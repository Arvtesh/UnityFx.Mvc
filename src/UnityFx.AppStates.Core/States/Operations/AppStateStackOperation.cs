// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A yieldable asynchronous state operation.
	/// </summary>
	/// <seealso href="https://blogs.msdn.microsoft.com/nikos/2011/03/14/how-to-implement-the-iasyncresult-design-pattern/"/>
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	internal abstract class AppStateStackOperation : IAppStateOperation<IAppState>, IAppStateOperationInfo, IExceptionAggregator, IAsyncResult, IDisposable
	{
		#region data

		private const int _statusDisposedFlag = 1;
		private const int _statusSynchronousFlag = 2;
		private const int _statusRunning = 0;
		private const int _statusCompleted = 4;
		private const int _statusFaulted = 8;
		private const int _statusCanceled = 16;
		private const int _typeMask = 0x7;

		private readonly int _id;
		private readonly IAppStateOperationOwner _owner;

		private static int _lastId;

		private AsyncCallback _asyncCallback;
		private object _asyncState;
		private EventWaitHandle _waitHandle;
		private List<Exception> _exceptions;
		private IAppState _result;

		private volatile int _status;

		#endregion

		#region interface

		public string DebuggerDisplay
		{
			get
			{
				var result = GetOperationName();
				var state = "Running";

				if ((_status & _statusCompleted) != 0)
				{
					state = "Completed";
				}
				else if ((_status & _statusFaulted) != 0)
				{
					state = "Faulted";
				}
				else if ((_status & _statusCanceled) != 0)
				{
					state = "Canceled";
				}

				result += ", State = ";
				result += state;

				if ((_status & _statusDisposedFlag) != 0)
				{
					result += ", Disposed";
				}

				return result;
			}
		}

		protected AppStateStackOperation(IAppStateOperationOwner owner, AppStateOperationType opType, AsyncCallback asyncCallback, object asyncState, string comment)
		{
			_id = (++_lastId << 3) | (int)opType;
			_owner = owner;
			_asyncCallback = asyncCallback;
			_asyncState = asyncState;

			var s = GetOperationName();

			if (!string.IsNullOrEmpty(comment))
			{
				s += ": " + comment;
			}

			TraceEvent(TraceEventType.Start, s);
		}

		protected bool TrySetResult(IAppState result, bool completedSynchronously = false)
		{
			if (TrySetStatus(_statusCompleted, completedSynchronously))
			{
				_result = result;
				OnCompleted();
				return true;
			}

			return false;
		}

		protected bool TrySetException(Exception e, bool completedSynchronously = false)
		{
			var status = e is OperationCanceledException ? _statusCanceled : _statusFaulted;

			if (TrySetStatus(status, completedSynchronously))
			{
				AddException(e);
				OnCompleted();
				return true;
			}

			return false;
		}

		protected bool TrySetCanceled(bool completedSynchronously = false)
		{
			if (TrySetStatus(_statusCanceled, completedSynchronously))
			{
				OnCompleted();
				return true;
			}

			return false;
		}

		protected void TraceError(string s)
		{
			_owner.TraceSource.TraceEvent(TraceEventType.Error, _id, GetOperationName() + ": " + s);
		}

		protected void TraceException(Exception e)
		{
			_owner.TraceSource.TraceData(TraceEventType.Error, _id, e);
		}

		protected void TraceEvent(TraceEventType eventType, string s)
		{
			_owner.TraceSource.TraceEvent(eventType, _id, s);
		}

		protected void TraceData(TraceEventType eventType, object data)
		{
			_owner.TraceSource.TraceData(eventType, _id, data);
		}

		protected void ThrowIfNotCompletedSuccessfully()
		{
			if ((_status & _statusCompleted) == 0)
			{
				throw new InvalidOperationException("The operation result is not available.");
			}
		}

		protected void ThrowIfDisposed()
		{
			if ((_status & _statusDisposedFlag) != 0)
			{
				throw new ObjectDisposedException(GetOperationName());
			}
		}

		protected string GetOperationTypeName()
		{
			return ((AppStateOperationType)(_id & _typeMask)).ToString();
		}

		protected string GetOperationName()
		{
			var result = (AppStateOperationType)(_id & _typeMask);
			return $"{result.ToString()} ({_id.ToString(CultureInfo.InvariantCulture)})";
		}

		#endregion

		#region IAppStateOperation

		public IAppState Result
		{
			get
			{
				ThrowIfDisposed();
				ThrowIfNotCompletedSuccessfully();
				return _result;
			}
		}

		public int Id => _id;
		public Exception Exception => _exceptions?[0];
		public IEnumerable<Exception> Exceptions => _exceptions;
		public bool IsCompletedSuccessfully => (_status & _statusCompleted) != 0;
		public bool IsFaulted => _status >= _statusFaulted;
		public bool IsCanceled => (_status & _statusCanceled) != 0;

		#endregion

		#region IAppStateOperationInfo

		public int OperationId => _id;
		public AppStateOperationType OperationType => (AppStateOperationType)(_id & _typeMask);
		public object UserState => _asyncState;

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

		#region IAsyncResult

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				ThrowIfDisposed();

				if (_waitHandle == null)
				{
					var done = IsCompleted;
					var mre = new ManualResetEvent(done);

					if (Interlocked.CompareExchange(ref _waitHandle, mre, null) != null)
					{
						// Another thread created this object's event; dispose the event we just created.
						mre.Close();
					}
					else if (!done && IsCompleted)
					{
						// We published the event as unset, but the operation has subsequently completed;
						// set the event state properly so that callers do not deadlock.
						_waitHandle.Set();
					}
				}

				return _waitHandle;
			}
		}

		public object AsyncState => _asyncState;
		public bool CompletedSynchronously => (_status & _statusSynchronousFlag) != 0;
		public bool IsCompleted => _status > _statusRunning;

		#endregion

		#region IEnumerator

		public object Current => null;
		public bool MoveNext() => _status == _statusRunning;
		public void Reset() => throw new NotSupportedException();

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if ((_status & _statusDisposedFlag) == 0)
			{
				if (!IsCompleted)
				{
					throw new InvalidOperationException("Cannot dispose non-completed operation.");
				}

				_status |= _statusDisposedFlag;
				_asyncCallback = null;
				_asyncState = null;
				_exceptions = null;
				_waitHandle?.Close();
				_waitHandle = null;
			}
		}

		#endregion

		#region implementation

		private void OnCompleted()
		{
			var s = GetOperationName() + (IsCompletedSuccessfully ? " completed" : " failed");

			TraceEvent(TraceEventType.Stop, s);

			_waitHandle?.Set();
			_asyncCallback?.Invoke(this);
			_asyncCallback = null;
		}

		private bool TrySetStatus(int newStatus, bool completedSynchronously)
		{
			if (_status < _statusCompleted)
			{
				if (completedSynchronously)
				{
					newStatus |= _statusSynchronousFlag;
				}

				return Interlocked.CompareExchange(ref _status, newStatus, _statusRunning) == _statusRunning;
			}

			return false;
		}

		#endregion
	}
}
