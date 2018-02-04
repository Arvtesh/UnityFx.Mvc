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

namespace UnityFx.AppStates
{
	/// <summary>
	/// Implementation of <see cref="IAppStateService"/>.
	/// </summary>
	internal sealed class AppStateManager : IAppStateService, IAppStateServiceSettings, IAppStateOperationOwner
	{
		#region data

		private const int _maxStackOperationsCount = 32;
		private const string _serviceName = "StateManager";

		private readonly IAppStateControllerFactory _controllerFactory;
		private readonly IAppStateViewFactory _viewManager;
		private readonly IServiceProvider _serviceProvider;
		private readonly SynchronizationContext _synchronizationContext;

		private readonly TraceSource _console;
		private readonly AppStateStack _states = new AppStateStack();
		private readonly Queue<AppStateStackOperation> _stackOperations = new Queue<AppStateStackOperation>();
		private readonly AppState _parentState;
		private readonly AppStateManager _parentStateManager;

		private AppStateStackOperation _currentOp;
		private bool _enabled;
		private bool _disposed;

		#endregion

		#region interface

		internal AppState ParentState => _parentState;

		internal AppStateStack StatesEx => _states;

		public AppStateManager(
			IAppStateControllerFactory controllerFactory,
			IAppStateViewFactory viewManager,
			IServiceProvider services)
			: this(SynchronizationContext.Current, controllerFactory, viewManager, services)
		{
		}

		public AppStateManager(
			IAppStateViewFactory viewManager,
			IServiceProvider services)
			: this(SynchronizationContext.Current, new AppStateControllerFactory(), viewManager, services)
		{
		}

		internal AppStateManager(
			SynchronizationContext syncContext,
			IAppStateControllerFactory controllerFactory,
			IAppStateViewFactory viewManager,
			IServiceProvider services)
		{
			Debug.Assert(controllerFactory != null);
			Debug.Assert(viewManager != null);
			Debug.Assert(services != null);

			_console = new TraceSource(_serviceName);
			_synchronizationContext = syncContext;
			_controllerFactory = controllerFactory;
			_viewManager = viewManager;
			_serviceProvider = services;
			_enabled = true;
		}

		internal AppStateManager(AppState parentState, AppStateManager parentStateManager)
		{
			Debug.Assert(parentState != null);
			Debug.Assert(parentStateManager != null);

			_console = parentStateManager._console;
			_synchronizationContext = parentStateManager._synchronizationContext;
			_controllerFactory = parentStateManager._controllerFactory;
			_viewManager = parentStateManager._viewManager;
			_parentState = parentState;
			_parentStateManager = parentStateManager;
			_serviceProvider = parentStateManager._serviceProvider;
			_enabled = parentState.Enabled;
		}

		internal AppStateManager CreateSubstateManager(AppState state, AppStateManager parentStateManager)
		{
			Debug.Assert(state != null);
			Debug.Assert(parentStateManager != null);
			Debug.Assert(!_disposed);

			return new AppStateManager(state, parentStateManager);
		}

		internal object CreateStateController(AppState state, Type controllerType)
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

		internal async Task PopAll(IExceptionAggregator ea)
		{
			Debug.Assert(!_disposed);

			// Signal all pending operations should complete ASAP.
			_cancellationSource.Cancel();

			// Wait for the current operation to finish.
			await WaitUntilAllOperationsAreProcessed();

			// Pop all states from the stack.
			TryDeactivateTopStateSafe(ea);
			await PopAllStatesInternal(ea);
		}

		internal bool TryActivateTopState()
		{
			Debug.Assert(!_disposed);

			if (_stackOperations.IsEmpty && _states.TryPeek(out var state))
			{
				state.Activate();
				return true;
			}

			return false;
		}

		internal bool TryActivateTopStateSafe(IExceptionAggregator ea)
		{
			Debug.Assert(!_disposed);

			try
			{
				return TryActivateTopState();
			}
			catch (Exception e)
			{
				ea.AddException(e);
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

		internal bool TryDeactivateTopStateSafe(IExceptionAggregator ea)
		{
			Debug.Assert(!_disposed);

			try
			{
				return TryDeactivateTopState();
			}
			catch (Exception e)
			{
				ea.AddException(e);
			}

			return false;
		}

		internal void SetEnabled()
		{
			_enabled = true;
			TryRunOperationProcessor();
		}

		internal void InvokeStatePushed(AppStateEventArgs args)
		{
			Debug.Assert(!_disposed);

			try
			{
				StatePushed?.Invoke(this, args);
			}
			catch (Exception e)
			{
				_console.TraceData(TraceEventType.Error, 0, e);
			}

			_parentStateManager?.InvokeStatePushed(args);
		}

		internal void InvokeStatePopped(AppStateEventArgs args)
		{
			Debug.Assert(!_disposed);

			try
			{
				StatePopped?.Invoke(this, args);
			}
			catch (Exception e)
			{
				_console.TraceData(TraceEventType.Error, 0, e);
			}

			_parentStateManager?.InvokeStatePopped(args);
		}

		internal void InvokeStateActivated(AppStateEventArgs args)
		{
			Debug.Assert(!_disposed);

			try
			{
				StateActivated?.Invoke(this, args);
			}
			catch (Exception e)
			{
				_console.TraceData(TraceEventType.Error, 0, e);
			}

			_parentStateManager?.InvokeStateActivated(args);
		}

		internal void InvokeStateDeactivated(AppStateEventArgs args)
		{
			Debug.Assert(!_disposed);

			try
			{
				StateDeactivated?.Invoke(this, args);
			}
			catch (Exception e)
			{
				_console.TraceData(TraceEventType.Error, 0, e);
			}

			_parentStateManager?.InvokeStateDeactivated(args);
		}

		#endregion

		#region IAppStateOperationOwner

		public TraceSource TraceSource => _console;

		#endregion

		#region IAppStateService

		public event EventHandler<AppStateEventArgs> StatePushed;
		public event EventHandler<AppStateEventArgs> StatePopped;
		public event EventHandler<AppStateEventArgs> StateActivated;
		public event EventHandler<AppStateEventArgs> StateDeactivated;

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

		public SourceSwitch TraceSwitch { get => _console.Switch; set => _console.Switch = value; }

		public TraceListenerCollection TraceListeners => _console.Listeners;

		#endregion

		#region IAppStateManager

		public event EventHandler<PushStateInitiatedEventArgs> PushStateInitiated;
		public event EventHandler<PushStateCompletedEventArgs> PushStateCompleted;
		public event EventHandler<PopStateInitiatedEventArgs> PopStateInitiated;
		public event EventHandler<PopStateCompletedEventArgs> PopStateCompleted;

		public IAppStateOperation<IAppState> PushStateAsync(PushOptions options, Type controllerType, object controllerArgs)
		{
			ThrowIfDisposed();
			ThrowIfInvalidControllerType(controllerType);
			ThrowIfTooManyOperations();

			return PushStateInternal(options, controllerType, controllerArgs, null, null);
		}

		public IAppStateOperation PopStateAsync(IAppState state)
		{
			ThrowIfDisposed();
			ThrowIfInvalidState(state);
			ThrowIfTooManyOperations();

			return PopStateInternal(state, null, null);
		}

#if UNITYFX_SUPPORT_TAP

		public Task<IAppState> PushStateTaskAsync(PushOptions options, Type controllerType, object controllerArgs)
		{
			ThrowIfDisposed();
			ThrowIfInvalidControllerType(controllerType);
			ThrowIfTooManyOperations();

			var tcs = new TaskCompletionSource<IAppState>();
			PushStateInternal(options, controllerType, controllerArgs, PushPopCompletionCallback, tcs);
			return tcs.Task;
		}

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

		#region IAppStateContainer

		public IAppStateStack States
		{
			get
			{
				ThrowIfDisposed();
				return _states;
			}
		}

		public IEnumerable<IAppState> GetStatesRecursive()
		{
			ThrowIfDisposed();

			var list = new List<IAppState>();
			GetStatesRecursive(list);
			return list;
		}

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

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;

				foreach (var state in _states)
				{
					state.Dispose();
				}

				_states.Clear();
			}
		}

		#endregion

		#region implementation

		////private async Task ProcessStackOperations()
		////{
		////	try
		////	{
		////		while (_stackOperations.TryDequeue(out var op))
		////		{
		////			if (op.CancellationToken.IsCancellationRequested)
		////			{
		////				op.TrySetCanceled();
		////				OnOperationComplete(op);
		////			}
		////			else
		////			{
		////				try
		////				{
		////					IAppState result = null;

		////					OnOperationStarted(op);

		////					if (op is PushStateOperation pushOp)
		////					{
		////						result = await ProcessPushOperation(pushOp);
		////					}
		////					else if (op is PopStateOperation popOp)
		////					{
		////						await ProcessPopOperation(popOp);
		////					}
		////					else
		////					{
		////						Debug.Fail("Unknown stack operation");
		////					}

		////					op.SetResult(result);
		////				}
		////				catch (OperationCanceledException)
		////				{
		////					op.TrySetCanceled();
		////				}
		////				catch (Exception e)
		////				{
		////					op.TrySetException(e);
		////				}
		////				finally
		////				{
		////					OnOperationComplete(op);
		////				}
		////			}
		////		}
		////	}
		////	catch (Exception e)
		////	{
		////		_console.TraceData(TraceEventType.Critical, 0, e);
		////	}
		////	finally
		////	{
		////		_stackOperationsProcessor = null;
		////	}
		////}

		////private async Task<IAppState> ProcessPushOperation(PushStateOperation op)
		////{
		////	Debug.Assert(op != null);

		////	AppState result = null;

		////	try
		////	{
		////		var options = op.Options;
		////		var ownerState = op.OwnerState;
		////		var cancellationToken = op.CancellationToken;
		////		var transition = op.Transition;

		////		TryDeactivateTopState();

		////		// Replace the specified state with the new one.
		////		if (options.HasFlag(PushOptions.Set))
		////		{
		////			result = new AppState(this, ownerState.OwnerState, op.ControllerType, op.ControllerArgs);
		////			await result.Push(cancellationToken);

		////			if (transition != null && !cancellationToken.IsCancellationRequested)
		////			{
		////				await transition.PlaySetTransition(ownerState, result, cancellationToken);
		////			}

		////			await PopStateInternal(ownerState, op);
		////		}

		////		// Remove all states from the stack and push the new one.
		////		else if (options.HasFlag(PushOptions.Reset))
		////		{
		////			// 1) Clear the state stack.
		////			await PopAllStatesInternal(op);

		////			// 2) Push the new state if no errors and the operation is not canceled.
		////			if (!cancellationToken.IsCancellationRequested)
		////			{
		////				result = new AppState(this, null, op.ControllerType, op.ControllerArgs);
		////				await result.Push(cancellationToken);

		////				// 3) Play transition animation if any.
		////				if (transition != null && !cancellationToken.IsCancellationRequested)
		////				{
		////					await transition.PlayPushTransition(result, cancellationToken);
		////				}
		////			}
		////		}

		////		// Just push the new state onto the stack.
		////		else
		////		{
		////			// 1) Push the new state onto the stack.
		////			result = new AppState(this, ownerState, op.ControllerType, op.ControllerArgs);
		////			await result.Push(cancellationToken);

		////			// 2) Play transition animation if any.
		////			if (transition != null && !cancellationToken.IsCancellationRequested)
		////			{
		////				if (ownerState != null)
		////				{
		////					await transition.PlayPushTransition(ownerState, result, cancellationToken);
		////				}
		////				else
		////				{
		////					await transition.PlayPushTransition(result, cancellationToken);
		////				}
		////			}
		////		}
		////	}
		////	catch
		////	{
		////		try
		////		{
		////			if (result != null)
		////			{
		////				await result.PopIfNotAlready(op);
		////			}
		////		}
		////		catch (Exception e)
		////		{
		////			// NOTE: Ignore any exceptions here.
		////			_console.TraceData(TraceEventType.Error, 0, e);
		////		}

		////		throw;
		////	}
		////	finally
		////	{
		////		TryActivateTopStateSafe(op);
		////	}

		////	return result;
		////}

		////private async Task ProcessPopOperation(PopStateOperation op)
		////{
		////	Debug.Assert(op != null);

		////	// Deativate the top state.
		////	TryDeactivateTopStateSafe(op);

		////	// Play pop transition.
		////	try
		////	{
		////		if (op.Transition != null && op.State != null)
		////		{
		////			await op.Transition.PlayPopTransition(op.State, op.CancellationToken);
		////		}
		////	}
		////	catch (Exception e)
		////	{
		////		op.AddException(e);
		////	}

		////	// Pop the state(s).
		////	try
		////	{
		////		if (op.State != null)
		////		{
		////			await PopStateInternal(op.State, op);
		////		}
		////		else
		////		{
		////			await PopAllStatesInternal(op);
		////		}
		////	}
		////	catch (Exception e)
		////	{
		////		op.AddException(e);
		////	}

		////	// Activate the new top state.
		////	TryActivateTopStateSafe(op);
		////}

		////private async Task PopAllStatesInternal(IExceptionAggregator ea)
		////{
		////	if (_states.Count > 0)
		////	{
		////		foreach (var s in _states.ToArray())
		////		{
		////			try
		////			{
		////				await s.Pop(ea);
		////			}
		////			catch (Exception e)
		////			{
		////				ea.AddException(e);
		////			}
		////		}
		////	}
		////}

		////private async Task PopStateInternal(AppState state, IExceptionAggregator ea)
		////{
		////	// Pop all dependent states (states that was pushed onto the stack by the state).
		////	if (_states.Count > 1)
		////	{
		////		foreach (var s in _states.ToArray())
		////		{
		////			if (s.OwnerState == state)
		////			{
		////				try
		////				{
		////					await s.Pop(ea);
		////				}
		////				catch (Exception e)
		////				{
		////					ea.AddException(e);
		////				}
		////			}
		////		}
		////	}

		////	// Release the state (and its substates). This will also remove state from the stack.
		////	try
		////	{
		////		await state.Pop(ea);
		////	}
		////	catch (Exception e)
		////	{
		////		ea.AddException(e);
		////	}
		////}

		////private void OnOperationStarted(IAppStateOperationInfo op)
		////{
		////	_console.TraceInformation(op.ToString());
		////}

		////private void OnOperationComplete(IAppStateOperationInfo op)
		////{
		////	if (op.IsFaulted)
		////	{
		////		foreach (var e in op.Exception.InnerExceptions)
		////		{
		////			_console.TraceData(TraceEventType.Error, 0, e);
		////		}
		////	}

		////	InvokeOperationCompleted(new AppStateOperationEventArgs(op, op.Target ?? op.Result));
		////}

		////private void InvokeOperationCompleted(AppStateOperationEventArgs args)
		////{
		////	try
		////	{
		////		StateOperationCompleted?.Invoke(this, args);
		////	}
		////	catch (Exception e)
		////	{
		////		_console.TraceData(TraceEventType.Error, 0, e);
		////	}

		////	_parentStateManager?.InvokeOperationCompleted(args);
		////}

		private AppStateStackOperation PushStateInternal(PushOptions options, Type controllerType, object controllerArgs, AsyncCallback asyncCallback, object asyncState)
		{
			Debug.Assert(controllerType != null);
			Debug.Assert(!_disposed);

			AppStateStackOperation result;

			if (options == PushOptions.Set)
			{
				result = new SetStateOperation(this, _parentState, controllerType, controllerArgs, asyncCallback, asyncState);
			}
			else if (options == PushOptions.Reset)
			{
				result = new ResetStateOperation(this, controllerType, controllerArgs, asyncCallback, asyncState);
			}
			else
			{
				result = new PushStateOperation(this, _parentState, controllerType, controllerArgs, asyncCallback, asyncState);
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
				result = new PopStateOperation(this, state as AppState, asyncCallback, asyncState);
			}
			else
			{
				result = new PopAllStatesOperation(this, asyncCallback, asyncState);
			}

			QueueOperation(result);
			return result;
		}

		private void QueueOperation(AppStateStackOperation op)
		{
			lock (_stackOperations)
			{
				_stackOperations.Enqueue(op);
			}

			if (_enabled)
			{
				if (_synchronizationContext != null)
				{
					_synchronizationContext.Post(TryRunOperationProcessor, this);
				}
				else
				{
					TryRunOperationProcessor();
				}
			}
		}

		private static void TryRunOperationProcessor(object args)
		{
			(args as AppStateManager).TryRunOperationProcessor();
		}

		private void TryRunOperationProcessor()
		{
			if (_enabled && _stackOperationsProcessor == null && !_stackOperations.IsEmpty)
			{
				var task = ProcessStackOperations();

				if (!task.IsCompleted)
				{
					_stackOperationsProcessor = task;
				}
			}
		}

#if UNITYFX_SUPPORT_TAP

		private static void PushPopCompletionCallback(IAsyncResult asyncResult)
		{
			var storeOp = asyncResult as IAppStateOperation<IAppState>;
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

		private void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetFullName());
			}
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
