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
	/// <seealso cref="AppState"/>
	/// <seealso cref="AppViewController"/>
	public class AppStateService : IAppStateService
	{
		#region data

		private readonly SynchronizationContext _synchronizationContext;
		private readonly IPresentableFactory _controllerFactory;
		private readonly IAppViewService _viewManager;
		private readonly IServiceProvider _serviceProvider;

		private readonly AppStateServiceSettings _settings;
		private readonly TreeListCollection<IAppState> _states;
		private readonly AsyncResultQueue<AsyncResult> _stackOperations;

		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// The service name.
		/// </summary>
		public const string Name = "StateManager";

		/// <summary>
		/// Gets a <see cref="System.Diagnostics.TraceSource"/> instance used by the service.
		/// </summary>
		/// <value>A <see cref="System.Diagnostics.TraceSource"/> instance used for tracing.</value>
		protected internal TraceSource TraceSource => _settings.TraceSource;

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
		public AppStateService(IAppViewService viewManager, IServiceProvider serviceProvider, IPresentableFactory controllerFactory, SynchronizationContext syncContext)
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
				_controllerFactory = new DefaultPresentableFactory(this, viewManager, serviceProvider);
			}

			_synchronizationContext = syncContext;
			_controllerFactory = controllerFactory;
			_viewManager = viewManager;
			_serviceProvider = serviceProvider;
			_settings = new AppStateServiceSettings();
			_states = new TreeListCollection<IAppState>();
			_stackOperations = new AsyncResultQueue<AsyncResult>(syncContext);
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
					if (!_stackOperations.IsEmpty)
					{
						foreach (var op in _stackOperations.Release())
						{
							op.Cancel();
						}
					}

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
				throw new ObjectDisposedException(Name);
			}
		}

		#endregion

		#region internals

		internal IPresentableFactory ControllerFactory => _controllerFactory;
		internal IAppViewService ViewManager => _viewManager;

		internal void DismissAllStates(ITraceable op)
		{
			while (_states.TryPeek(out var state))
			{
				state.DismissInternal(op);
				state.Dispose();
			}
		}

		internal void DismissStateChildren(ITraceable op, AppState state)
		{
			foreach (var s in state.Children.Reverse())
			{
				state.DismissInternal(op);
				state.Dispose();
			}
		}

		internal bool TryActivateTopState(ITraceable op)
		{
			Debug.Assert(!_disposed);
			Debug.Assert(op != null);

			if (_states.TryPeek(out var state) && _stackOperations.Count <= 1)
			{
				return state.TryActivate(op);
			}

			return false;
		}

		internal bool TryDeactivateTopState(ITraceable op)
		{
			Debug.Assert(!_disposed);
			Debug.Assert(op != null);

			if (_states.TryPeek(out var state))
			{
				return state.TryDeactivate(op);
			}

			return false;
		}

		internal void AddState(AppState state)
		{
			_states.Add(state);
		}

		internal void RemoveState(AppState state)
		{
			_states.Remove(state);
		}

		internal IAsyncOperation<IPresentable> PresentAsync(AppState state, Type controllerType, PresentArgs args)
		{
			ThrowIfDisposed();
			ThrowIfInvalidControllerType(controllerType);

			////var result = new PresentOperation<IPresentable>(this, state, controllerType, args);
			////QueueOperation(result);
			////return result;

			throw new NotImplementedException();
		}

		internal IAsyncOperation<T> PresentAsync<T>(AppState state, PresentArgs args) where T : IPresentable
		{
			ThrowIfDisposed();
			ThrowIfInvalidControllerType(typeof(T));

			////var result = new PresentOperation<T>(this, state, typeof(T), args);
			////QueueOperation(result);
			////return result;

			throw new NotImplementedException();
		}

		internal IAsyncOperation DismissAsync(AppState state)
		{
			ThrowIfDisposed();

			var result = new DismissOperation(this, state);
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
		public IAppStateServiceSettings Settings => _settings;

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
#if NET35
		public ICollection<IAppState> States => _states;
#else
		public IReadOnlyCollection<IAppState> States => _states;
#endif

		#endregion

		#region IPresenter

		/// <inheritdoc/>
		public IAsyncOperation<IPresentable> PresentAsync(Type controllerType, PresentArgs args)
		{
			return PresentAsync(null, controllerType, args);
		}

		/// <inheritdoc/>
		public IAsyncOperation<TController> PresentAsync<TController>(PresentArgs args) where TController : IPresentable
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

			if (!typeof(IPresentable).IsAssignableFrom(controllerType))
			{
				throw new ArgumentException($"A state controller is expected to implement " + typeof(IPresentable).Name, nameof(controllerType));
			}
		}

		#endregion
	}
}
