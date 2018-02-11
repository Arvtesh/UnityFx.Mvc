// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
#if UNITYFX_SUPPORT_TAP
using System.Threading.Tasks;
#endif
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Implementation of <see cref="IAppStateService"/>.
	/// </summary>
	public class AppStateService : IAppStateService, IAppStateServiceSettings
	{
		#region data

		private const int _maxStackOperationsCount = 32;
		private const string _serviceName = "StateManager";

		private readonly IAppStateControllerFactory _controllerFactory;
		private readonly IAppStateViewFactory _viewManager;
		private readonly IServiceProvider _serviceProvider;
		private readonly SynchronizationContext _synchronizationContext;

		private readonly TraceSource _console;
		private readonly AppStateStack _states;
		private readonly AsyncResultQueue<AppStateStackOperation> _stackOperations;
		private readonly AppState _parentState;
		private readonly AppStateService _parentStateManager;

		private bool _enabled;
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Gets the <see cref="System.Diagnostics.TraceSource"/> instance used by the service.
		/// </summary>
		/// <value>A <see cref="System.Diagnostics.TraceSource"/> instance used for tracing.</value>
		protected internal TraceSource TraceSource => _console;

		/// <summary>
		/// Initializes a new instance of the <see cref="AppStateService"/> class.
		/// </summary>
		/// <param name="controllerFactory"></param>
		/// <param name="viewManager"></param>
		/// <param name="services"></param>
		public AppStateService(
			IAppStateControllerFactory controllerFactory,
			IAppStateViewFactory viewManager,
			IServiceProvider services)
			: this(SynchronizationContext.Current, controllerFactory, viewManager, services)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppStateService"/> class.
		/// </summary>
		/// <param name="viewManager"></param>
		/// <param name="services"></param>
		public AppStateService(
			IAppStateViewFactory viewManager,
			IServiceProvider services)
			: this(SynchronizationContext.Current, new AppStateControllerFactory(), viewManager, services)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppStateService"/> class.
		/// </summary>
		/// <param name="syncContext"></param>
		/// <param name="controllerFactory"></param>
		/// <param name="viewManager"></param>
		/// <param name="services"></param>
		public AppStateService(
			SynchronizationContext syncContext,
			IAppStateControllerFactory controllerFactory,
			IAppStateViewFactory viewManager,
			IServiceProvider services)
		{
			Debug.Assert(controllerFactory != null);
			Debug.Assert(viewManager != null);
			Debug.Assert(services != null);

			_console = new TraceSource(_serviceName);
			_states = new AppStateStack();
			_stackOperations = new AsyncResultQueue<AppStateStackOperation>(syncContext);
			_synchronizationContext = syncContext;
			_controllerFactory = controllerFactory;
			_viewManager = viewManager;
			_serviceProvider = services;
			_enabled = true;
		}

		internal AppStateService(AppState parentState, AppStateService parentStateManager)
		{
			Debug.Assert(parentState != null);
			Debug.Assert(parentStateManager != null);

			_console = parentStateManager._console;
			_states = new AppStateStack();
			_stackOperations = new AsyncResultQueue<AppStateStackOperation>(parentStateManager._synchronizationContext);
			_synchronizationContext = parentStateManager._synchronizationContext;
			_controllerFactory = parentStateManager._controllerFactory;
			_viewManager = parentStateManager._viewManager;
			_parentState = parentState;
			_parentStateManager = parentStateManager;
			_serviceProvider = parentStateManager._serviceProvider;
			_enabled = parentState.Enabled;
			_stackOperations.Suspended = !_enabled;
		}

		internal AppStateService CreateSubstateManager(AppState state, AppStateService parentStateManager)
		{
			Debug.Assert(state != null);
			Debug.Assert(parentStateManager != null);
			Debug.Assert(!_disposed);

			return new AppStateService(state, parentStateManager);
		}

		internal IAppStateController CreateStateController(AppState state, Type controllerType)
		{
			Debug.Assert(state != null);
			Debug.Assert(controllerType != null);
			Debug.Assert(!_disposed);

			return _controllerFactory.CreateController(controllerType, state, _serviceProvider);
		}

		internal IAppStateView CreateView(AppState state)
		{
			Debug.Assert(state != null);
			Debug.Assert(!_disposed);

			return _viewManager.CreateView(state.FullName, state.GetPrevView());
		}

		internal bool TryActivateTopState()
		{
			Debug.Assert(!_disposed);

			if (_stackOperations.Count == 0 && _states.TryPeek(out var state))
			{
				state.Activate();
				return true;
			}

			return false;
		}

		internal bool TryDeactivateTopState()
		{
			Debug.Assert(!_disposed);

			if (_states.TryPeek(out var state))
			{
				state.Deactivate();
				return true;
			}

			return false;
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
				_disposed = true;

				foreach (var state in _states)
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
				throw new ObjectDisposedException(GetFullName());
			}
		}

		#endregion

		#region internals

		internal AppState ParentState => _parentState;

		internal AppStateStack StatesEx => _states;

		internal void SetEnabled()
		{
			_enabled = true;
			_stackOperations.Suspended = false;
		}

		#endregion

		#region IAppStateService

		/// <inheritdoc/>
		public IAppStateServiceSettings Settings
		{
			get
			{
				ThrowIfDisposed();
				return this;
			}
		}

		#endregion

		#region IAppStateServiceSettings

		/// <inheritdoc/>
		public SourceSwitch TraceSwitch { get => _console.Switch; set => _console.Switch = value; }

		/// <inheritdoc/>
		public TraceListenerCollection TraceListeners => _console.Listeners;

		#endregion

		#region IAppStateManager

		/// <inheritdoc/>
		public event EventHandler<PushStateInitiatedEventArgs> PushStateInitiated;

		/// <inheritdoc/>
		public event EventHandler<PushStateCompletedEventArgs> PushStateCompleted;

		/// <inheritdoc/>
		public event EventHandler<PopStateInitiatedEventArgs> PopStateInitiated;

		/// <inheritdoc/>
		public event EventHandler<PopStateCompletedEventArgs> PopStateCompleted;

		/// <inheritdoc/>
		public IAppStateStack States
		{
			get
			{
				ThrowIfDisposed();
				return _states;
			}
		}

		/// <inheritdoc/>
		public IEnumerable<IAppState> GetStatesRecursive()
		{
			ThrowIfDisposed();

			var list = new List<IAppState>();
			GetStatesRecursive(list);
			return list;
		}

		/// <inheritdoc/>
		public void GetStatesRecursive(ICollection<IAppState> states)
		{
			ThrowIfDisposed();

			if (states == null)
			{
				throw new ArgumentNullException(nameof(states));
			}

			foreach (var state in _states)
			{
				states.Add(state);
				state.GetStatesRecursive(states);
			}
		}

		/// <inheritdoc/>
		public IAsyncOperation<IAppState> PushStateAsync(PushOptions options, Type controllerType, object controllerArgs)
		{
			ThrowIfDisposed();
			ThrowIfInvalidControllerType(controllerType);
			ThrowIfTooManyOperations();

			return PushStateInternal(options, controllerType, controllerArgs, null, null);
		}

		/// <inheritdoc/>
		public IAsyncOperation PopStateAsync(IAppState state)
		{
			ThrowIfDisposed();
			ThrowIfInvalidState(state);
			ThrowIfTooManyOperations();

			return PopStateInternal(state, null, null);
		}

#if UNITYFX_SUPPORT_TAP

		/// <inheritdoc/>
		public Task<IAppState> PushStateTaskAsync(PushOptions options, Type controllerType, object controllerArgs)
		{
			ThrowIfDisposed();
			ThrowIfInvalidControllerType(controllerType);
			ThrowIfTooManyOperations();

			var tcs = new TaskCompletionSource<IAppState>();
			PushStateInternal(options, controllerType, controllerArgs, PushPopCompletionCallback, tcs);
			return tcs.Task;
		}

		/// <inheritdoc/>
		public Task PopStateTaskAsync(IAppState state)
		{
			ThrowIfDisposed();
			ThrowIfInvalidState(state);
			ThrowIfTooManyOperations();

			var tcs = new TaskCompletionSource<IAppState>();
			PopStateInternal(state, PushPopCompletionCallback, tcs);
			return tcs.Task;
		}

#endif

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

		private AppStateStackOperation PushStateInternal(PushOptions options, Type controllerType, object controllerArgs, AsyncCallback asyncCallback, object asyncState)
		{
			Debug.Assert(controllerType != null);
			Debug.Assert(!_disposed);

			AppStateStackOperation result;

			if (options == PushOptions.Set)
			{
				result = new SetStateOperation(_console, _parentState, controllerType, controllerArgs, asyncCallback, asyncState);
			}
			else if (options == PushOptions.Reset)
			{
				result = new ResetStateOperation(_console, controllerType, controllerArgs, asyncCallback, asyncState);
			}
			else
			{
				result = new PushStateOperation(_console, _parentState, controllerType, controllerArgs, asyncCallback, asyncState);
			}

			QueueOperation(result);
			return result;
		}

		private AppStateStackOperation PopStateInternal(IAppState state, AsyncCallback asyncCallback, object asyncState)
		{
			Debug.Assert(!_disposed);

			AppStateStackOperation result;

			if (state != null)
			{
				result = new PopStateOperation(_console, state as AppState, asyncCallback, asyncState);
			}
			else
			{
				result = new PopAllStatesOperation(_console, asyncCallback, asyncState);
			}

			QueueOperation(result);
			return result;
		}

		private void QueueOperation(AppStateStackOperation op)
		{
			_stackOperations.Add(op);
		}

#if UNITYFX_SUPPORT_TAP

		private static void PushPopCompletionCallback(IAsyncResult asyncResult)
		{
			var storeOp = asyncResult as IAsyncOperation<IAppState>;
			var tcs = asyncResult.AsyncState as TaskCompletionSource<IAppState>;

			if (storeOp.IsCompletedSuccessfully)
			{
				tcs.TrySetResult(storeOp.Result);
			}
			else if (storeOp.IsCanceled)
			{
				tcs.TrySetCanceled();
			}
			else
			{
				tcs.TrySetException(storeOp.Exception);
			}
		}

#endif

		private string GetFullName()
		{
			if (_parentState != null)
			{
				return _parentState.FullName + '.' + _serviceName;
			}

			return _serviceName;
		}

		private void ThrowIfTooManyOperations()
		{
			if (_stackOperations.Count > _maxStackOperationsCount)
			{
				throw new InvalidOperationException($"Operation cannot be scheduled because maximum number of simultaneous stack operations ({_maxStackOperationsCount}) is exceeded.");
			}
		}

		private void ThrowIfInvalidState(IAppState state)
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

			if (controllerType.IsValueType)
			{
				throw new ArgumentException($"A state instance cannot be value-type", nameof(controllerType));
			}
		}

		#endregion
	}
}
