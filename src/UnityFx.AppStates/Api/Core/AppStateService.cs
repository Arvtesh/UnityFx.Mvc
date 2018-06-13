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
		private readonly IAppViewControllerFactory _controllerFactory;
		private readonly IAppViewService _viewManager;
		private readonly IServiceProvider _serviceProvider;

		private readonly AppStateServiceSettings _settings;
		private readonly AppStateCollection _states;
		private readonly AsyncResultQueue<AppOperation> _stackOperations;

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
		/// /// <param name="viewManager"></param>
		/// <param name="services"></param>
		public AppStateService(IAppViewService viewManager, IServiceProvider services)
			: this(viewManager, services, SynchronizationContext.Current)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppStateService"/> class.
		/// </summary>
		/// <param name="syncContext"></param>
		/// <param name="services"></param>
		/// <param name="viewManager"></param>
		public AppStateService(
			IAppViewService viewManager,
			IServiceProvider services,
			SynchronizationContext syncContext)
		{
			Debug.Assert(viewManager != null);
			Debug.Assert(services != null);

			_synchronizationContext = syncContext;
			_controllerFactory = new AppViewControllerFactory(this, viewManager, services);
			_viewManager = viewManager;
			_serviceProvider = services;
			_settings = new AppStateServiceSettings();
			_states = new AppStateCollection();
			_stackOperations = new AsyncResultQueue<AppOperation>(syncContext);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppStateService"/> class.
		/// </summary>
		/// <param name="syncContext"></param>
		/// <param name="viewManager"></param>
		/// <param name="services"></param>
		/// <param name="controllerFactory"></param>
		public AppStateService(
			IAppViewControllerFactory controllerFactory,
			IAppViewService viewManager,
			IServiceProvider services,
			SynchronizationContext syncContext)
		{
			Debug.Assert(controllerFactory != null);
			Debug.Assert(viewManager != null);
			Debug.Assert(services != null);

			_synchronizationContext = syncContext;
			_controllerFactory = controllerFactory;
			_viewManager = viewManager;
			_serviceProvider = services;
			_settings = new AppStateServiceSettings();
			_states = new AppStateCollection();
			_stackOperations = new AsyncResultQueue<AppOperation>(syncContext);
		}

		/// <summary>
		/// Releases unmanaged resources used by the service.
		/// </summary>
		/// <param name="disposing">Should be <see langword="true"/> if the method is called from <see cref="Dispose()"/>; <see langword="false"/> otherwise.</param>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="ThrowIfDisposed"/>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				// 1) Stop operation processing.
				_disposed = true;
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
				foreach (var state in _states.GetEnumerableLifo())
				{
					state.Dispose();
				}

				_states.Clear();
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

		internal IAppViewControllerFactory ControllerFactory => _controllerFactory;
		internal IAppViewService ViewManager => _viewManager;

		internal void DismissAllStates(AppOperation op)
		{
			while (_states.TryPeek(out var state))
			{
				state.DismissInternal(op);
				state.Dispose();
			}
		}

		internal void DismissStateChildren(AppOperation op, AppState state)
		{
			foreach (var s in _states.GetChildren(state).Reverse())
			{
				state.DismissInternal(op);
				state.Dispose();
			}
		}

		internal bool TryActivateTopState(AppOperation op)
		{
			Debug.Assert(!_disposed);
			Debug.Assert(op != null);

			if (_states.TryPeek(out var state) && _stackOperations.Count <= 1)
			{
				return state.TryActivate(op);
			}

			return false;
		}

		internal bool TryDeactivateTopState(AppOperation op)
		{
			Debug.Assert(!_disposed);
			Debug.Assert(op != null);

			if (_states.TryPeek(out var state))
			{
				return state.TryDeactivate(op);
			}

			return false;
		}

		internal IAsyncOperation<AppViewController> PresentAsync(AppViewController parentController, Type controllerType, PresentOptions options, PresentArgs args)
		{
			ThrowIfDisposed();

			var result = new PresentOperation(this, parentController, controllerType, options, args);
			QueueOperation(result);
			return result;
		}

		internal IAsyncOperation<AppViewController> PresentAsync(AppState state, Type controllerType, PresentOptions options, PresentArgs args)
		{
			ThrowIfDisposed();
			ThrowIfInvalidControllerType(controllerType);

			var result = new PresentOperation(this, state, controllerType, options, args);
			QueueOperation(result);
			return result;
		}

		internal IAsyncOperation DismissAsync(AppViewController controller)
		{
			ThrowIfDisposed();

			var result = new DismissOperation(this, controller);
			QueueOperation(result);
			return result;
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
		public AppStateServiceSettings Settings => _settings;

		/// <inheritdoc/>
		public bool IsBusy => !_stackOperations.IsEmpty;

		/// <inheritdoc/>
		public AppStateCollection States => _states;

		#endregion

		#region IPresenter

		/// <inheritdoc/>
		public IAsyncOperation<AppViewController> PresentAsync(Type controllerType, PresentOptions options, PresentArgs args)
		{
			ThrowIfDisposed();

			return PresentAsync(default(AppState), controllerType, options, args);
		}

		/// <inheritdoc/>
		public IAsyncOperation<AppViewController> PresentAsync(Type controllerType, PresentArgs args)
		{
			ThrowIfDisposed();

			return PresentAsync(default(AppState), controllerType, PresentOptions.None, args);
		}

		/// <inheritdoc/>
		public IAsyncOperation<TController> PresentAsync<TController>(PresentOptions options, PresentArgs args) where TController : AppViewController
		{
			return PresentAsync(typeof(TController), options, args) as IAsyncOperation<TController>;
		}

		/// <inheritdoc/>
		public IAsyncOperation<TController> PresentAsync<TController>(PresentArgs args) where TController : AppViewController
		{
			return PresentAsync(typeof(TController), args) as IAsyncOperation<TController>;
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

		private void QueueOperation(AppOperation op)
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

			if (!controllerType.IsSubclassOf(typeof(AppViewController)))
			{
				throw new ArgumentException($"A state controller is expected to inherit " + typeof(AppViewController).Name, nameof(controllerType));
			}
		}

		#endregion
	}
}
