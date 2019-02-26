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
		private readonly TraceSource _traceSource = new TraceSource(ServiceName);
		private readonly TreeListCollection<PresentableProxy> _controllers = new TreeListCollection<PresentableProxy>();

		private SendOrPostCallback _startCallback;
		private int _idCounter;
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
					foreach (var c in _controllers.Reverse())
					{
						c.Dismiss();
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

		internal void DismissAllControllers()
		{
			while (_controllers.TryPeek(out var controller))
			{
				controller.Dispose();
			}
		}

		internal IPresentResult Present(PresentableProxy parent, Type controllerType, PresentArgs args)
		{
			Debug.Assert(args != null);

			ThrowIfDisposed();
			ThrowIfInvalidControllerType(controllerType);

			var id = PrePresent(parent, args);
			var result = new PresentableProxy(this, parent, controllerType, args, id);

			try
			{
				AddController(result);
				Present(result);
			}
			catch
			{
				RemoveController(result);
				throw;
			}

			PostPresent(result);
			return result;
		}

		internal IPresentResult<T> Present<T>(PresentableProxy parent, PresentArgs args) where T : class, IPresentable
		{
			Debug.Assert(args != null);

			ThrowIfDisposed();
			ThrowIfInvalidControllerType(typeof(T));

			var id = PrePresent(parent, args);
			var result = new PresentableProxy<T>(this, parent, typeof(T), args, id);

			try
			{
				AddController(result);
				Present(result);
			}
			catch
			{
				RemoveController(result);
				throw;
			}

			PostPresent(result);
			return result;
		}

		internal void Dismiss(PresentableProxy controller)
		{
			ThrowIfDisposed();

			// TODO
			OnDismissInitiated(controller.Controller, result);
			QueueOperation(result, PresentOptions.None);
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
		public IPresentResult Present(Type controllerType)
		{
			return Present(null, controllerType, PresentArgs.Default);
		}

		/// <inheritdoc/>
		public IPresentResult Present(Type controllerType, PresentArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException(nameof(args));
			}

			return Present(null, controllerType, args);
		}

		/// <inheritdoc/>
		public IPresentResult<TController> Present<TController>() where TController : class, IViewController
		{
			return Present<TController>(null, PresentArgs.Default);
		}

		/// <inheritdoc/>
		public IPresentResult<TController> Present<TController>(PresentArgs args) where TController : class, IViewController
		{
			if (args == null)
			{
				throw new ArgumentNullException(nameof(args));
			}

			return Present<TController>(null, args);
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

		private int PrePresent(PresentableProxy parent, PresentArgs args)
		{
			Debug.Assert(args != null);
			Debug.Assert(!_disposed);

			var id = ++_idCounter;

			if (parent != null)
			{
				parent.TryDeactivate();
			}
			else if (_controllers.TryPeek(out var topController))
			{
				topController.TryDeactivate();
			}

			return id;
		}

		private void Present(PresentableProxy controller)
		{
			Debug.Assert(controller != null);
			Debug.Assert(!_disposed);

			controller.OnPresent();
		}

		private void PostPresent(PresentableProxy controller)
		{
			Debug.Assert(controller != null);
			Debug.Assert(!_disposed);

			if (controller == _controllers.Last)
			{
				controller.TryActivate();
			}
		}

		private void AddController(PresentableProxy controller)
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

		private void RemoveController(PresentableProxy controller)
		{
			Debug.Assert(controller != null);
			Debug.Assert(!_disposed);

			_controllers.Remove(controller);
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
