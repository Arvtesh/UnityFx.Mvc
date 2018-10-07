// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
#if !NET35
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A manager of application states (<see cref="IAppState"/>).
	/// </summary>
	/// <threadsafety static="true" instance="false"/>
	/// <seealso cref="IAppState"/>
	/// <seealso cref="IViewController"/>
	public class AppStateService : IAppStateService
	{
		#region data

		private readonly IServiceProvider _serviceProvider;
		private readonly SynchronizationContext _synchronizationContext;
		private readonly AppStateServiceSettings _config;
		private readonly TraceSource _traceSource = new TraceSource(ServiceName);
		private readonly AppStateCollection _states = new AppStateCollection();
#if NET35
		private readonly List<AsyncResult> _ops = new List<AsyncResult>();
#else
		private readonly ConcurrentQueue<AsyncResult> _ops = new ConcurrentQueue<AsyncResult>();
#endif

		private AsyncResult _currentOp;
		private SendOrPostCallback _startCallback;
		private Action<IAsyncOperation> _completionCallback;
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// The service name.
		/// </summary>
		public const string ServiceName = "StateManager";

		/// <summary>
		/// Gets a <see cref="System.Diagnostics.TraceSource"/> instance used by the service.
		/// </summary>
		/// <value>A <see cref="System.Diagnostics.TraceSource"/> instance used for tracing.</value>
		protected internal TraceSource TraceSource => _traceSource;

		/// <summary>
		/// Gets a <see cref="System.Threading.SynchronizationContext"/> instance used by the service.
		/// </summary>
		protected internal SynchronizationContext SynchronizationContext => _synchronizationContext;

		/// <summary>
		/// Gets a <see cref="IServiceProvider"/> instance used by the service.
		/// </summary>
		protected internal IServiceProvider ServiceProvider => _serviceProvider;

		/// <summary>
		/// Initializes a new instance of the <see cref="AppStateService"/> class.
		/// </summary>
		/// <param name="serviceProvider"></param>
		public AppStateService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
			_synchronizationContext = SynchronizationContext.Current;
			_config = new AppStateServiceSettings(_traceSource);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppStateService"/> class.
		/// </summary>
		/// <param name="syncContext"></param>
		/// <param name="serviceProvider"></param>
		public AppStateService(IServiceProvider serviceProvider, SynchronizationContext syncContext)
		{
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
			_synchronizationContext = syncContext ?? throw new ArgumentNullException(nameof(syncContext));
			_config = new AppStateServiceSettings(_traceSource);
		}

		/// <summary>
		/// Called when a new present operation has been initiated.
		/// </summary>
		protected internal virtual void OnPresentInitiated(PresentArgs args, IAsyncOperation op)
		{
			PresentInitiated?.Invoke(this, new PresentInitiatedEventArgs(args, op.Id, op.AsyncState));
		}

		/// <summary>
		/// Called when a present operation has completed.
		/// </summary>
		protected internal virtual void OnPresentCompleted(IAppState state, IViewController controller, IAsyncOperation op)
		{
			PresentCompleted?.Invoke(this, new PresentCompletedEventArgs(state, controller, op.Id, op.AsyncState, op.Exception, op.IsCanceled));
		}

		/// <summary>
		/// Called when a new dismiss operation has been initiated.
		/// </summary>
		protected internal virtual void OnDismissInitiated(IAppState state, IViewController controller, IAsyncOperation op)
		{
			DismissInitiated?.Invoke(this, new DismissInitiatedEventArgs(state, controller, op.Id, op.AsyncState));
		}

		/// <summary>
		/// Called when a dismiss operation has completed.
		/// </summary>
		protected internal virtual void OnDismissCompleted(IAppState state, IViewController controller, IAsyncOperation op)
		{
			DismissCompleted?.Invoke(this, new DismissCompletedEventArgs(state, controller, op.Id, op.AsyncState, op.Exception, op.IsCanceled));
		}

		/// <summary>
		/// Releases unmanaged resources used by the service.
		/// </summary>
		/// <param name="disposing">Should be <see langword="true"/> if the method is called from <see cref="Dispose()"/>; <see langword="false"/> otherwise.</param>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="ThrowIfDisposed"/>
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				_disposed = true;

				if (disposing)
				{
					// 1) Stop operation processing.
#if NET35
					lock (_ops)
					{
						foreach (var op in _ops)
						{
							op.RemoveCompletionCallback(_completionCallback);
							op.Cancel();
						}

						_ops.Clear();
					}
#else
					while (_ops.TryDequeue(out var op))
					{
						op.RemoveCompletionCallback(_completionCallback);
						op.Cancel();
					}
#endif

					_currentOp = null;

					// 2) Dispose child states.
					foreach (var state in _states.Reverse())
					{
						state.Dispose();
					}

					_states.Clear();
				}
			}
		}

		/// <summary>
		/// Throws an <see cref="ObjectDisposedException"/> if the instance is already disposed.
		/// </summary>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="Dispose(bool)"/>
		protected void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(ServiceName);
			}
		}

		#endregion

		#region internals

		internal void TraceException(Exception e)
		{
			Debug.Assert(e != null);
			Debug.Assert(!_disposed);

			var opId = _currentOp?.Id ?? 0;
			_traceSource.TraceEvent(TraceEventType.Error, opId, e.ToString());
		}

		internal void TraceEvent(TraceEventType eventType, string s)
		{
			Debug.Assert(s != null);
			Debug.Assert(!_disposed);

			var opId = _currentOp?.Id ?? 0;
			_traceSource.TraceEvent(eventType, opId, s);
		}

		internal void TraceData(TraceEventType eventType, object data)
		{
			Debug.Assert(!_disposed);

			var opId = _currentOp?.Id ?? 0;
			_traceSource.TraceData(eventType, opId, data);
		}

		internal void AddState(AppState state)
		{
			Debug.Assert(state != null);
			Debug.Assert(!_disposed);

			var parent = state.Parent;

			if (parent != null)
			{
				var prev = parent;
				var next = parent.Next;

				while (next != null && next.IsChildOf(parent))
				{
					prev = next;
					next = prev.Next;
				}

				_states.Add(state, prev);
			}
			else
			{
				_states.Add(state);
			}
		}

		internal void RemoveState(AppState state)
		{
			Debug.Assert(state != null);
			Debug.Assert(!_disposed);

			_states.Remove(state);
		}

		internal IAsyncOperation<IViewController> PresentAsync(AppState parentState, Type controllerType, PresentArgs args)
		{
			Debug.Assert(args != null);
			ThrowIfDisposed();
			ThrowIfInvalidControllerType(controllerType);

			var result = new PresentOperation<IViewController>(this, parentState, controllerType, args);
			OnPresentInitiated(args, result);
			QueueOperation(result);
			return result;
		}

		internal IAsyncOperation<T> PresentAsync<T>(AppState parentState, PresentArgs args) where T : class, IViewController
		{
			Debug.Assert(args != null);
			ThrowIfDisposed();
			ThrowIfInvalidControllerType(typeof(T));

			var result = new PresentOperation<T>(this, parentState, typeof(T), args);
			OnPresentInitiated(args, result);
			QueueOperation(result);
			return result;
		}

		internal IAsyncOperation DismissAsync(AppState state)
		{
			ThrowIfDisposed();

			var result = new DismissOperation(this, state, null);
			OnDismissInitiated(state, state.Controller, result);
			QueueOperation(result);
			return result;
		}

		#endregion

		#region IAppStateService

		/// <inheritdoc/>
		public event EventHandler<PresentInitiatedEventArgs> PresentInitiated;

		/// <inheritdoc/>
		public event EventHandler<PresentCompletedEventArgs> PresentCompleted;

		/// <inheritdoc/>
		public event EventHandler<DismissInitiatedEventArgs> DismissInitiated;

		/// <inheritdoc/>
		public event EventHandler<DismissCompletedEventArgs> DismissCompleted;

		/// <inheritdoc/>
		public bool IsBusy
		{
			get
			{
#if NET35
				return _ops.Count != 0;
#else
				return !_ops.IsEmpty;
#endif
			}
		}

		/// <inheritdoc/>
		public IAppState ActiveState
		{
			get
			{
				var result = _states.Last;

				if (result != null && result.IsActive)
				{
					return result;
				}

				return null;
			}
		}

		/// <inheritdoc/>
		public IAppStateCollection States => _states;

		/// <inheritdoc/>
		public IAppStateServiceSettings Settings => _config;

		#endregion

		#region IPresenter

		/// <inheritdoc/>
		public IAsyncOperation<IViewController> PresentAsync(Type controllerType)
		{
			return PresentAsync(null, controllerType, PresentArgs.Default);
		}

		/// <inheritdoc/>
		public IAsyncOperation<IViewController> PresentAsync(Type controllerType, PresentArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException(nameof(args));
			}

			return PresentAsync(null, controllerType, args);
		}

		/// <inheritdoc/>
		public IAsyncOperation<TController> PresentAsync<TController>() where TController : class, IViewController
		{
			return PresentAsync<TController>(null, PresentArgs.Default);
		}

		/// <inheritdoc/>
		public IAsyncOperation<TController> PresentAsync<TController>(PresentArgs args) where TController : class, IViewController
		{
			if (args == null)
			{
				throw new ArgumentNullException(nameof(args));
			}

			return PresentAsync<TController>(null, args);
		}

		#endregion

		#region IDisposable

		/// <inheritdoc/>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		#region implementation

		private void QueueOperation(AsyncResult op)
		{
			Debug.Assert(op != null);
			Debug.Assert(!op.IsStarted);

#if NET35
			lock (_ops)
			{
				_ops.Add(op);
				TryStart();
			}
#else
			_ops.Enqueue(op);
			TryStart();
#endif
		}

		private void TryStart()
		{
			if (!_disposed)
			{
				if (_synchronizationContext == null || _synchronizationContext == SynchronizationContext.Current)
				{
					TryStartUnsafe();
				}
				else
				{
					if (_startCallback == null)
					{
						_startCallback = OnStartCallback;
					}

					_synchronizationContext.Post(_startCallback, null);
				}
			}
		}

		private void TryStartUnsafe()
		{
			// This is supposed to be UI thread.
			if (!_disposed)
			{
				if (_completionCallback == null)
				{
					_completionCallback = OnCompletedCallback;
				}

#if NET35
				while (_ops.Count > 0)
				{
					var firstOp = _ops[0];

					if (firstOp.IsCompleted)
					{
						_ops.RemoveAt(0);
						_currentOp = null;
					}
					else
					{
						firstOp.AddCompletionCallback(_completionCallback);
						firstOp.TryStart();
						_currentOp = firstOp;
						break;
					}
				}
#else
				while (_ops.TryPeek(out var firstOp))
				{
					if (firstOp.IsCompleted)
					{
						_ops.TryDequeue(out firstOp);
						_currentOp = null;
					}
					else
					{
						firstOp.AddCompletionCallback(_completionCallback);
						firstOp.TryStart();
						_currentOp = firstOp;
						break;
					}
				}
#endif
			}
		}

		private void OnStartCallback(object args)
		{
#if NET35
			lock (_ops)
			{
				TryStartUnsafe();
			}
#else
			TryStartUnsafe();
#endif
		}

		private void OnCompletedCallback(IAsyncOperation op)
		{
#if NET35
			lock (_ops)
			{
				TryStartUnsafe();
			}
#else
			TryStartUnsafe();
#endif
		}

		private void ThrowIfInvalidArgs(PresentArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException(nameof(args));
			}
		}

		private void ThrowIfInvalidState(AppState state)
		{
			if (state == null)
			{
				throw new ArgumentNullException(nameof(state));
			}

			if (!_states.Contains(state))
			{
				throw new InvalidOperationException("The state does not belong to the manager.");
			}
		}

		private static void ThrowIfInvalidControllerType(Type controllerType)
		{
			if (controllerType == null)
			{
				throw new ArgumentNullException(nameof(controllerType));
			}

			if (controllerType.IsAbstract)
			{
				throw new ArgumentException($"Cannot instantiate abstract type {controllerType.Name}", nameof(controllerType));
			}

			if (!typeof(IViewController).IsAssignableFrom(controllerType))
			{
				throw new ArgumentException($"A state controller is expected to implement " + typeof(IViewController).Name, nameof(controllerType));
			}
		}

		#endregion
	}
}
