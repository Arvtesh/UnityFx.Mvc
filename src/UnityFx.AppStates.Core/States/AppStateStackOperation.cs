// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UnityFx.AppStates
{
	internal abstract class AppStateStackOperation : IAppStateOperation, IExceptionAggregator, IAsyncResult, IDisposable
	{
		#region data

		private const int _statusDisposedFlag = 1;
		private const int _statusSynchronousFlag = 2;
		private const int _statusRunning = 0;
		private const int _statusCompleted = 4;
		private const int _statusFaulted = 8;
		private const int _statusCanceled = 16;

		private readonly int _id;

		private static int _lastId;

		private AsyncCallback _asyncCallback;
		private object _asyncState;
		private EventWaitHandle _waitHandle;
		private List<Exception> _exceptions;

		private volatile int _status;

		#endregion

		#region interface

		public IAppStateTransition Transition { get; }

		public CancellationToken CancellationToken { get; }

		public AppStateStackOperation(IAppStateTransition transition, CancellationToken ct)
		{
			Transition = transition;
			CancellationToken = ct;
		}

		protected bool TrySetCompleted(bool completedSynchronously = false)
		{
			if (TrySetStatus(_statusCompleted, completedSynchronously))
			{
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

		#endregion

		#region IAppStateOperationInfo

		public int Id => _id;
		public abstract AppStateOperationType Type { get; }
		public abstract object Args { get; }
		public abstract IAppState Result { get; }
		public Exception Exception => _exceptions?[0];
		public IReadOnlyCollection<Exception> Exceptions => _exceptions;
		public bool IsCompletedSuccessfully => (_status & _statusCompleted) != 0;
		public bool IsFaulted => _status >= _statusFaulted;
		public bool IsCanceled => (_status & _statusCanceled) != 0;

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

		private string GetOperationName()
		{
			return string.Empty;
		}

		#endregion
	}
}
