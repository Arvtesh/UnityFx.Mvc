﻿// Copyright (c) Alexander Bogarsukov.
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
using UnityFx.Async.Extensions;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A present service implementation (<see cref="IPresentService"/>).
	/// </summary>
	/// <threadsafety static="true" instance="false"/>
	/// <seealso cref="IViewController"/>
	public class PresentService : IPresentService
	{
		#region data

		private readonly IServiceProvider _serviceProvider;
		private readonly SynchronizationContext _synchronizationContext;
		private readonly AppStateServiceSettings _config;
		private readonly TraceSource _traceSource = new TraceSource(ServiceName);
		private readonly TreeListCollection<ViewControllerProxy> _controllers = new TreeListCollection<ViewControllerProxy>();
#if NET35
		private readonly List<AsyncResult> _ops = new List<AsyncResult>();
#else
		private readonly ConcurrentQueue<AsyncResult> _ops = new ConcurrentQueue<AsyncResult>();
#endif

		private IAsyncOperation _currentOp;
		private SendOrPostCallback _startCallback;
		private Action<IAsyncOperation> _completionCallback;
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// The service name.
		/// </summary>
		public const string ServiceName = "MvcManager";

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
		/// Initializes a new instance of the <see cref="PresentService"/> class.
		/// </summary>
		/// <param name="serviceProvider"></param>
		public PresentService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
			_synchronizationContext = SynchronizationContext.Current;
			_config = new AppStateServiceSettings(_traceSource);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentService"/> class.
		/// </summary>
		/// <param name="serviceProvider"></param>
		/// <param name="syncContext"></param>
		public PresentService(IServiceProvider serviceProvider, SynchronizationContext syncContext)
		{
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
			_synchronizationContext = syncContext;
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
		protected internal virtual void OnPresentCompleted(IViewController controller, IAsyncOperation op)
		{
			PresentCompleted?.Invoke(this, new PresentCompletedEventArgs(controller, op.Id, op.AsyncState, op.Exception, op.IsCanceled));
		}

		/// <summary>
		/// Called when a new dismiss operation has been initiated.
		/// </summary>
		protected internal virtual void OnDismissInitiated(IViewController controller, IAsyncOperation op)
		{
			DismissInitiated?.Invoke(this, new DismissInitiatedEventArgs(controller, op.Id, op.AsyncState));
		}

		/// <summary>
		/// Called when a dismiss operation has completed.
		/// </summary>
		protected internal virtual void OnDismissCompleted(IViewController controller, IAsyncOperation op)
		{
			DismissCompleted?.Invoke(this, new DismissCompletedEventArgs(controller, op.Id, op.AsyncState, op.Exception, op.IsCanceled));
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
					foreach (var state in _controllers.Reverse())
					{
						state.Dispose();
					}

					_controllers.Clear();
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

		internal void TraceStart(IAsyncOperation op)
		{
			Debug.Assert(op != null);
			Debug.Assert(!_disposed);

			Trace.CorrelationManager.StartLogicalOperation(op);

			_traceSource.TraceEvent(TraceEventType.Start, op.Id, op.ToString());
			_currentOp = op;
		}

		internal void TraceStop(IAsyncOperation op)
		{
			Debug.Assert(op != null);
			Debug.Assert(op.IsCompleted);
			Debug.Assert(op == _currentOp);
			Debug.Assert(!_disposed);

			_currentOp = null;

			switch (op.Status)
			{
				case AsyncOperationStatus.RanToCompletion:
					_traceSource.TraceEvent(TraceEventType.Stop, op.Id, op.ToString());
					break;

				case AsyncOperationStatus.Faulted:
					_traceSource.TraceData(TraceEventType.Error, op.Id, op.Exception);
					_traceSource.TraceEvent(TraceEventType.Stop, op.Id, op.ToString() + " (faulted)");
					break;

				case AsyncOperationStatus.Canceled:
					_traceSource.TraceEvent(TraceEventType.Stop, op.Id, op.ToString() + " (canceled)");
					break;
			}

			Trace.CorrelationManager.StopLogicalOperation();
		}

		internal void TraceException(Exception e)
		{
			Debug.Assert(e != null);
			Debug.Assert(!_disposed);

			var opId = _currentOp?.Id ?? 0;
			_traceSource.TraceData(TraceEventType.Error, opId, e.ToString());
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

		internal void AddController(ViewControllerProxy controller)
		{
			Debug.Assert(controller != null);
			Debug.Assert(!_disposed);

			var parent = controller.Parent;

			if (parent != null)
			{
				var prev = parent;
				var next = parent.Next;

				while (next != null && next.IsChildOf(parent))
				{
					prev = next;
					next = prev.Next;
				}

				_controllers.Add(controller, prev);
			}
			else
			{
				_controllers.Add(controller);
			}
		}

		internal void RemoveController(ViewControllerProxy controller)
		{
			Debug.Assert(controller != null);
			Debug.Assert(!_disposed);

			_controllers.Remove(controller);
		}

		internal void DismissAllControllers()
		{
			while (_controllers.TryPeek(out var controller))
			{
				controller.Dispose();
			}
		}

		internal void InvokePresentStarted(Type controllerType, PresentArgs args, IAsyncOperation op)
		{
			Debug.Assert(op != null);

			TryDeactivateTopState();
		}

		internal void InvokePresentCompleted(ViewControllerProxy controllerProxy, IAsyncOperation op)
		{
			Debug.Assert(op != null);
			Debug.Assert(op.IsCompleted);

			// Do not rethrow exceptions because the operation has already completed and we have no way to pass it to user code at this point.
			try
			{
				OnPresentCompleted(controllerProxy?.Controller, op);
			}
			catch (Exception e)
			{
				TraceException(e);
			}

			try
			{
				TryActivateTopState();
			}
			catch (Exception e)
			{
				TraceException(e);
			}
		}

		internal void InvokeDismissStarted(ViewControllerProxy controllerProxy, IAsyncOperation op)
		{
			Debug.Assert(op != null);

			TryDeactivateTopState();
		}

		internal void InvokeDismissCompleted(ViewControllerProxy controllerProxy, IAsyncOperation op)
		{
			Debug.Assert(op != null);
			Debug.Assert(op.IsCompleted);

			// Do not rethrow exceptions because the operation has already completed and we have no way to pass it to user code at this point.
			try
			{
				OnDismissCompleted(controllerProxy?.Controller, op);
			}
			catch (Exception e)
			{
				TraceException(e);
			}

			try
			{
				_controllers.Remove(controllerProxy);
				TryActivateTopState();
			}
			catch (Exception e)
			{
				TraceException(e);
			}
		}

		internal IAsyncOperation<IViewController> PresentAsync(ViewControllerProxy parent, Type controllerType, PresentArgs args)
		{
			Debug.Assert(args != null);
			ThrowIfDisposed();
			ThrowIfInvalidControllerType(controllerType);

			var result = new PresentOperation<IViewController>(this, parent, controllerType, args);
			OnPresentInitiated(args, result);
			QueueOperation(result, args.Options);
			return result;
		}

		internal IAsyncOperation<T> PresentAsync<T>(ViewControllerProxy parent, PresentArgs args) where T : class, IViewController
		{
			Debug.Assert(args != null);
			ThrowIfDisposed();
			ThrowIfInvalidControllerType(typeof(T));

			var result = new PresentOperation<T>(this, parent, typeof(T), args);
			OnPresentInitiated(args, result);
			QueueOperation(result, args.Options);
			return result;
		}

		internal IAsyncOperation DismissAsync(ViewControllerProxy controller)
		{
			ThrowIfDisposed();

			var result = new DismissOperation(this, controller, null);
			OnDismissInitiated(controller.Controller, result);
			QueueOperation(result, PresentOptions.None);
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
		public IServiceProvider ServiceProvider
		{
			get
			{
				return _serviceProvider;
			}
		}

		/// <inheritdoc/>
		public IViewController ActiveController
		{
			get
			{
				var result = _controllers.Last;

				if (result != null && result.IsActive)
				{
					return result.Controller;
				}

				return null;
			}
		}

		/// <inheritdoc/>
		public IPresentServiceSettings Settings => _config;

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

		#region ICommandTarget

		/// <summary>
		/// Invokes a command. An implementation might choose to ignore the command, in this case the method should return <see langword="false"/>.
		/// </summary>
		/// <param name="commandName">Name of the command to invoke.</param>
		/// <param name="args">Command-specific arguments.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="commandName"/> is <see langword="null"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the service is disposed.</exception>
		/// <returns>Returns <see langword="true"/> if the command has been handles; <see langword="false"/> otherwise.</returns>
		public bool InvokeCommand(string commandName, object args)
		{
			ThrowIfDisposed();

			if (commandName == null)
			{
				throw new ArgumentNullException(nameof(commandName));
			}

			foreach (var controllerProxy in _controllers.Reverse())
			{
				if (controllerProxy.InvokeCommand(commandName, args))
				{
					return true;
				}
				else if (controllerProxy.IsModal)
				{
					// Do not forward commands past first modal controller in the stack.
					break;
				}
			}

			return false;
		}

		#endregion

		#region ISynchronizeInvoke

		/// <summary>
		/// Gets a value indicating whether the caller must call <see cref="Invoke(Delegate, object[])"/> when calling the service.
		/// </summary>
		/// <seealso cref="BeginInvoke(Delegate, object[])"/>
		/// <seealso cref="EndInvoke(IAsyncResult)"/>
		/// <seealso cref="Invoke(Delegate, object[])"/>
		public bool InvokeRequired
		{
			get
			{
				return _synchronizationContext != null && _synchronizationContext != SynchronizationContext.Current;
			}
		}

		/// <summary>
		/// Asynchronously executes the delegate on the thread that created the service.
		/// </summary>
		/// <param name="method">A delegate to a method that takes parameters of the same number and type that are contained in <paramref name="args"/>.</param>
		/// <param name="args">An array of type <see cref="object"/> to pass as arguments to the given <paramref name="method"/>. This can be <see langword="null"/> if no arguments are needed.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="method"/> is <see langword="null"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the service is disposed.</exception>
		/// <returns>An <see cref="IAsyncResult"/> interface that represents the asynchronous operation started by calling the <paramref name="method"/>.</returns>
		/// <seealso cref="EndInvoke(IAsyncResult)"/>
		/// <seealso cref="Invoke(Delegate, object[])"/>
		/// <seealso cref="InvokeRequired"/>
		public IAsyncResult BeginInvoke(Delegate method, object[] args)
		{
			ThrowIfDisposed();

			if (method == null)
			{
				throw new ArgumentNullException(nameof(method));
			}

			if (_synchronizationContext == null)
			{
				throw new NotSupportedException();
			}

			var op = new AsyncCompletionSource<object>(this);
			PostToSyncContext(method, args, op);
			return op;
		}

		/// <summary>
		/// Waits until the process started by calling <see cref="BeginInvoke(Delegate, object[])"/> completes, and then returns the value generated by the process.
		/// </summary>
		/// <param name="result">An <see cref="IAsyncResult"/> interface that represents the asynchronous operation started by calling <see cref="BeginInvoke(Delegate, object[])"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="result"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the <paramref name="result"/> is not created with <see cref="BeginInvoke(Delegate, object[])"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the service is disposed.</exception>
		/// <returns>An <see cref="object"/> that represents the return value generated by the asynchronous operation.</returns>
		/// <seealso cref="BeginInvoke(Delegate, object[])"/>
		/// <seealso cref="Invoke(Delegate, object[])"/>
		/// <seealso cref="InvokeRequired"/>
		public object EndInvoke(IAsyncResult result)
		{
			ThrowIfDisposed();

			if (result == null)
			{
				throw new ArgumentNullException(nameof(result));
			}

			if (result.AsyncState != this)
			{
				throw new InvalidOperationException();
			}

			if (result is AsyncCompletionSource<object> op)
			{
				using (op)
				{
					return op.Join();
				}
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// Synchronously executes the delegate on the thread that created the service and marshals the call to the creating thread.
		/// </summary>
		/// <param name="method">A delegate that contains a method to call, in the context of the thread for the service.</param>
		/// <param name="args">An array of type <see cref="object"/> that represents the arguments to pass to the given <paramref name="method"/>. This can be <see langword="null"/> if no arguments are needed.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="method"/> is <see langword="null"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the service is disposed.</exception>
		/// <returns>An <see cref="object"/> that represents the return value from the delegate being invoked, or <see langword="null"/> if the delegate has no return value.</returns>
		/// <seealso cref="BeginInvoke(Delegate, object[])"/>
		/// <seealso cref="EndInvoke(IAsyncResult)"/>
		/// <seealso cref="InvokeRequired"/>
		public object Invoke(Delegate method, object[] args)
		{
			ThrowIfDisposed();

			if (method == null)
			{
				throw new ArgumentNullException(nameof(method));
			}

			if (_synchronizationContext == null || _synchronizationContext == SynchronizationContext.Current)
			{
				return method.DynamicInvoke(args);
			}
			else
			{
				using (var op = new AsyncCompletionSource<object>())
				{
					PostToSyncContext(method, args, op);
					return op.Join();
				}
			}
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

		private void QueueOperation(AsyncResult op, PresentOptions options)
		{
			Debug.Assert(op != null);
			Debug.Assert(!op.IsStarted);

#if NET35

			lock (_ops)
			{
				_ops.Add(op);
				TryStart(op, (options & PresentOptions.ExcecuteAsync) != 0);
			}

#else

			_ops.Enqueue(op);
			TryStart(op, (options & PresentOptions.ExcecuteAsync) != 0);

#endif
		}

		private void TryStart(AsyncResult op, bool forceAsync)
		{
			// NOTE: This can be called from any thread.
			// NOTE: For net35 _ops should be locked at this point.
			if (!_disposed)
			{
				if ((_synchronizationContext == null || _synchronizationContext == SynchronizationContext.Current) && !forceAsync)
				{
					TryStartUnsafe();
				}
				else
				{
					if (_startCallback == null)
					{
						_startCallback = OnStartCallback;
					}

					if (op is IAppStateOperation op2)
					{
						op2.SetCompletedAsynchronously();
					}

					_synchronizationContext.Post(_startCallback, null);
				}
			}
		}

		private void TryStartUnsafe()
		{
			// NOTE: This is supposed to be UI thread.
			// NOTE: For net35 _ops should be locked at this point.
			Debug.Assert(_synchronizationContext == null || SynchronizationContext.Current == _synchronizationContext);

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
					}
					else
					{
						firstOp.AddCompletionCallback(_completionCallback);
						firstOp.TryStart();
						break;
					}
				}

#else

				while (_ops.TryPeek(out var firstOp))
				{
					if (firstOp.IsCompleted)
					{
						_ops.TryDequeue(out firstOp);
					}
					else
					{
						firstOp.AddCompletionCallback(_completionCallback);
						firstOp.TryStart();
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

		private void TryActivateTopState()
		{
			if (_controllers.TryPeek(out var state) && !state.IsActive)
			{
				(state as IPresentableEvents).OnActivate();
			}
		}

		private void TryDeactivateTopState()
		{
			if (_controllers.TryPeek(out var state) && state.IsActive)
			{
				(state as IPresentableEvents).OnDeactivate();
			}
		}

		private void PostToSyncContext(Delegate method, object[] args, AsyncCompletionSource<object> op)
		{
			Debug.Assert(method != null);
			Debug.Assert(op != null);
			Debug.Assert(_synchronizationContext != null);

			_synchronizationContext.Post(() =>
			{
				try
				{
					var result = method.DynamicInvoke(args);
					op.SetResult(result);
				}
				catch (Exception e)
				{
					op.SetException(e);
				}
			});
		}

		private void ThrowIfInvalidArgs(PresentArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException(nameof(args));
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