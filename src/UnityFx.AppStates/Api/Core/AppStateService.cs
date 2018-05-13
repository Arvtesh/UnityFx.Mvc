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
	public class AppStateService : IAppStateService, IDisposable
	{
		#region data

		private readonly SynchronizationContext _synchronizationContext;
		private readonly IAppControllerFactory _controllerFactory;
		private readonly IAppViewService _viewManager;
		private readonly IServiceProvider _serviceProvider;

		private readonly AppStateServiceSettings _settings;
		private readonly AppStateCollection _states;
		private readonly AsyncResultQueue<AppStateOperation> _stackOperations;

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
			_stackOperations = new AsyncResultQueue<AppStateOperation>(syncContext);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppStateService"/> class.
		/// </summary>
		/// <param name="syncContext"></param>
		/// <param name="viewManager"></param>
		/// <param name="services"></param>
		/// <param name="controllerFactory"></param>
		public AppStateService(
			IAppControllerFactory controllerFactory,
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
			_stackOperations = new AsyncResultQueue<AppStateOperation>(syncContext);
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

		internal IAppControllerFactory ControllerFactory => _controllerFactory;
		internal IAppViewService ViewManager => _viewManager;

		internal void PopStates(IAppStateOperationInfo op, AppState targetState)
		{
			while (_states.TryPeek(out var state))
			{
				if (state == targetState)
				{
					break;
				}
				else
				{
					state.Pop(op);
				}
			}
		}

		internal void PopStateDependencies(IAppStateOperationInfo op, AppState state)
		{
			foreach (var s in _states.ToArrayLifo())
			{
				if (s.Parent == state)
				{
					s.Pop(op);
				}
			}
		}

		internal bool TryActivateTopState(IAppStateOperationInfo op)
		{
			Debug.Assert(!_disposed);
			Debug.Assert(op != null);

			if (_states.TryPeek(out var state) && _stackOperations.Count <= 1)
			{
				state.Activate(op);
				return true;
			}

			return false;
		}

		internal bool TryDeactivateTopState(IAppStateOperationInfo op)
		{
			Debug.Assert(!_disposed);
			Debug.Assert(op != null);

			if (_states.TryPeek(out var state))
			{
				state.Deactivate(op);
				return true;
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

			AppStateOperation result;

			if ((options & PresentOptions.DismissAllStates) == PresentOptions.DismissAllStates)
			{
				result = new SetStateOperation(this, null, controllerType, args);
			}
			else if ((options & PresentOptions.DismissCurrentState) != 0)
			{
				result = new SetStateOperation(this, state, controllerType, args);
			}
			else
			{
				result = new PresentOperation(this, state, controllerType, options, args);
			}

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

		/// <inheritdoc/>
		public IAsyncOperation<AppViewController> PresentAsync(Type controllerType, PresentArgs args)
		{
			return PresentAsync(default(AppState), controllerType, PresentOptions.None, args);
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

		private void QueueOperation(AppStateOperation op)
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
			AppViewController.ValidateControllerType(controllerType);
		}

		#endregion
	}
}
