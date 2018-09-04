// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A manager of application states (<see cref="AppState"/>).
	/// </summary>
	/// <threadsafety static="true" instance="false"/>
	/// <seealso cref="IAppState"/>
	/// <seealso cref="IViewController"/>
	public class AppStateService : IAppStateService
	{
		#region data

		private readonly TraceSource _traceSource;
		private readonly SynchronizationContext _synchronizationContext;
		private readonly IViewControllerFactory _controllerFactory;
		private readonly IAppViewService _viewManager;
		private readonly IServiceProvider _serviceProvider;

		private readonly AppStateServiceSettings _settings;
		private readonly AppStateCollection _states;
		private readonly AsyncResultQueue<AsyncResult> _stackOperations;

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
		/// <param name="viewManager"></param>
		/// <param name="serviceProvider"></param>
		public AppStateService(IAppViewService viewManager, IServiceProvider serviceProvider)
			: this(viewManager, serviceProvider, null, SynchronizationContext.Current)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppStateService"/> class.
		/// </summary>
		/// <param name="syncContext"></param>
		/// <param name="serviceProvider"></param>
		/// <param name="viewManager"></param>
		public AppStateService(IAppViewService viewManager, IServiceProvider serviceProvider, SynchronizationContext syncContext)
			: this(viewManager, serviceProvider, null, syncContext)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppStateService"/> class.
		/// </summary>
		/// <param name="syncContext"></param>
		/// <param name="viewManager"></param>
		/// <param name="serviceProvider"></param>
		/// <param name="controllerFactory"></param>
		public AppStateService(IAppViewService viewManager, IServiceProvider serviceProvider, IViewControllerFactory controllerFactory, SynchronizationContext syncContext)
		{
			if (viewManager == null)
			{
				throw new ArgumentNullException(nameof(viewManager));
			}

			if (serviceProvider == null)
			{
				throw new ArgumentNullException(nameof(serviceProvider));
			}

			if (controllerFactory == null)
			{
				controllerFactory = new DefaultViewControllerFactory(serviceProvider);
			}

			_traceSource = new TraceSource(ServiceName);
			_synchronizationContext = syncContext;
			_controllerFactory = controllerFactory;
			_viewManager = viewManager;
			_serviceProvider = serviceProvider;
			_settings = new AppStateServiceSettings();
			_states = new AppStateCollection();
			_stackOperations = new AsyncResultQueue<AsyncResult>(syncContext);
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
		protected internal virtual void OnPresentCompleted(IViewController result, IAsyncOperation op)
		{
			PresentCompleted?.Invoke(this, new PresentCompletedEventArgs(result, op.Id, op.AsyncState, op.Exception, op.IsCanceled));
		}

		/// <summary>
		/// Called when a new dismiss operation has been initiated.
		/// </summary>
		protected internal virtual void OnDismissInitiated(IDismissable target, IAsyncOperation op)
		{
			DismissInitiated?.Invoke(this, new DismissInitiatedEventArgs(target, op.Id, op.AsyncState));
		}

		/// <summary>
		/// Called when a dismiss operation has completed.
		/// </summary>
		protected internal virtual void OnDismissCompleted(IDismissable target, IAsyncOperation op)
		{
			DismissCompleted?.Invoke(this, new DismissCompletedEventArgs(target, op.Id, op.AsyncState, op.Exception, op.IsCanceled));
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
					_stackOperations.Suspended = true;

					// 2) Cancel pending operations.
					_stackOperations.Cancel();

					// 3) Dispose child states.
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

		internal IViewControllerFactory ControllerFactory => _controllerFactory;
		internal IAppViewService ViewManager => _viewManager;

		internal void AddState(AppState state)
		{
			Debug.Assert(state != null);

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

			_states.Remove(state);
		}

		internal IAsyncOperation<IViewController> PresentAsync(AppState parentState, Type controllerType, PresentArgs args)
		{
			ThrowIfDisposed();
			ThrowIfInvalidControllerType(controllerType);

			var result = new PresentOperation<IViewController>(this, parentState, controllerType, args ?? PresentArgs.Default);
			OnPresentInitiated(args, result);
			QueueOperation(result);
			return result;
		}

		internal IAsyncOperation<T> PresentAsync<T>(AppState parentState, PresentArgs args) where T : class, IViewController
		{
			ThrowIfDisposed();
			ThrowIfInvalidControllerType(typeof(T));

			var result = new PresentOperation<T>(this, parentState, typeof(T), args ?? PresentArgs.Default);
			OnPresentInitiated(args, result);
			QueueOperation(result);
			return result;
		}

		internal IAsyncOperation DismissAsync(AppState state)
		{
			ThrowIfDisposed();

			var result = new DismissOperation(this, state, null);
			OnDismissInitiated(state, result);
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
		public SourceSwitch TraceSwitch { get => _traceSource.Switch; set => _traceSource.Switch = value; }

		/// <inheritdoc/>
		public TraceListenerCollection TraceListeners => _traceSource.Listeners;

		/// <inheritdoc/>
		public bool IsBusy => !_stackOperations.IsEmpty;

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
		public IAppStateServiceSettings Settings => _settings;

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
			_stackOperations.Add(op);
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
