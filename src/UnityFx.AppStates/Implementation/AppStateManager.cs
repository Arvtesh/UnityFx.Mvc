// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace UnityFx.App
{
	/// <summary>
	/// Implementation of <see cref="IAppStateService"/>.
	/// </summary>
	internal sealed class AppStateManager : IAppStateService, IAppStateServiceSettings
	{
		#region data

		private const int _maxStackOperationsCount = 32;
		private const string _serviceName = "StateManager";

		private readonly AppStateStack _states = new AppStateStack();
		private readonly Queue<AppStateStackOperation> _stackOperations = new Queue<AppStateStackOperation>();
		private readonly CancellationTokenSource _cancellationSource = new CancellationTokenSource();

		private readonly TraceSource _console;
		private readonly SynchronizationContext _synchronizationContext;
		private readonly AppState _parentState;
		private readonly IAppViewFactory _viewManager;
		private readonly IServiceProvider _services;

		private Task _stackOperationsProcessor;
		private bool _disposed;

		#endregion

		#region interface

		internal IServiceProvider Services => _services;

		internal AppStateStack StatesEx => _states;

		internal AppState ParentState => _parentState;

		internal AppStateManager(SynchronizationContext syncContext, IAppViewFactory viewManager, IServiceProvider services)
		{
			Debug.Assert(viewManager != null);
			Debug.Assert(services != null);

			_console = new TraceSource(_serviceName);
			_synchronizationContext = syncContext;
			_viewManager = viewManager;
			_services = services;
		}

		internal AppStateManager(AppState parentState, TraceSource console, SynchronizationContext syncContext, IAppViewFactory viewManager, IServiceProvider services)
		{
			Debug.Assert(parentState != null);
			Debug.Assert(console != null);
			Debug.Assert(viewManager != null);
			Debug.Assert(services != null);

			_console = console;
			_synchronizationContext = syncContext;
			_viewManager = viewManager;
			_parentState = parentState;
			_services = services;
		}

		internal AppStateManager CreateSubstateManager(AppState state)
		{
			Debug.Assert(state != null);
			ThrowIfDisposed();

			return new AppStateManager(state, _console, _synchronizationContext, _viewManager, _services);
		}

		internal IAppView CreateView(AppState state)
		{
			Debug.Assert(state != null);
			ThrowIfDisposed();

			var exclusive = !state.Flags.HasFlag(AppStateFlags.Popup);
			var insertAfterView = default(IAppView);

			// TODO
			return _viewManager.CreateView(state.Name, exclusive, insertAfterView, state);
		}

		internal Task<IAppState> PushState(AppState ownerState, Type controllerType, PushOptions options, object stateArgs)
		{
			ThrowIfDisposed();
			ThrowIfInvalidControllerType(controllerType);

			var op = new AppStatePushOperation(options, ownerState, null, _cancellationSource.Token, controllerType, stateArgs);
			AddStackOperation(op);
			RunStackOperation(op);
			return op.Task;
		}

		internal Task PopState(AppState state)
		{
			Debug.Assert(state != null);
			ThrowIfDisposed();

			var op = new AppStatePopOperation(state, null, _cancellationSource.Token);
			AddStackOperation(op);
			RunStackOperation(op);
			return op.Task;
		}

		internal async Task PopAll()
		{
			ThrowIfDisposed();

			// Signal all pending operations should complete asap.
			_cancellationSource.Cancel();

			// Wait for the current operatino to finish.
			if (_stackOperationsProcessor != null)
			{
				await _stackOperationsProcessor;
			}

			// Pop all states from the stack.
			if (_states.TryPeek(out var topState))
			{
				topState.Deactivate();

				foreach (var state in _states.ToArray())
				{
					await state.Pop(_cancellationSource.Token);
				}
			}
		}

		internal void ActivateTopState()
		{
			if (_states.TryPeek(out var state))
			{
				if (state.Activate())
				{
					InvokeStateActivated(state);
				}
			}
		}

		internal void DeactivateTopState()
		{
			if (_states.TryPeek(out var state))
			{
				if (state.Deactivate())
				{
					InvokeStateDeactivated(state);
				}
			}
		}

		#endregion

		#region IAppStateService

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

		public Task<IAppState> PushStateAsync<T>(PushOptions options, object args) where T : class, IAppStateController
		{
			ThrowIfDisposed();
			return PushState(_parentState, typeof(T), options, args);
		}

		public Task<IAppState> PushStateAsync(Type controllerType, PushOptions options, object args)
		{
			ThrowIfDisposed();
			return PushState(_parentState, controllerType, options, args);
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;

				// TODO
			}
		}

		#endregion

		#region implementation

		private void RunOpProcessor(object state)
		{
			if (_stackOperationsProcessor == null && _stackOperations.Count > 0 && !_cancellationSource.IsCancellationRequested)
			{
				var task = ProcessStackOperations();

				if (!task.IsCompleted)
				{
					_stackOperationsProcessor = task;
				}
			}
		}

		private void AddStackOperation(AppStateStackOperation op)
		{
			lock (_stackOperations)
			{
				if (_stackOperations.Count > _maxStackOperationsCount)
				{
					throw new InvalidOperationException($"Operation cannot be scheduled because maximum number of simultaneous stack operations ({_maxStackOperationsCount}) is exceeded.");
				}

				if (_stackOperations.Count > 0)
				{
					if (op.Operation == StackOperation.Push)
					{
						// TODO
					}
					else if (op.Operation == StackOperation.Pop)
					{
						// TODO
					}
				}

				_stackOperations.Enqueue(op);
			}

			InvokeOperationInitiated(op);
		}

		private void RunStackOperation(AppStateStackOperation op)
		{
			if (_synchronizationContext != null)
			{
				_synchronizationContext.Post(RunOpProcessor, null);
			}
			else
			{
				RunOpProcessor(null);
			}
		}

		private async Task ProcessStackOperations()
		{
			try
			{
				var firstOp = true;

				// NOTE: _stackOperations may be modified from inside the loop, cannot use iterators.
				while (_stackOperations.Count > 0)
				{
					AppStateStackOperation op;

					lock (_stackOperations)
					{
						op = _stackOperations.Dequeue();
					}

					if (CanExecuteOperation(op))
					{
						if (firstOp)
						{
							firstOp = false;
							DeactivateTopState();
						}

						try
						{
							op.CancellationToken.ThrowIfCancellationRequested();

							if (op.Operation == StackOperation.Push)
							{
								var state = await ProcessPushOperation(op as AppStatePushOperation);
								op.SetResult(state);
							}
							else if (op.Operation == StackOperation.Pop)
							{
								await ProcessPopOperation(op as AppStatePopOperation);
								op.SetResult(null);
							}
						}
						catch (OperationCanceledException e)
						{
							op.TrySetCanceled();
							_console.TraceData(TraceEventType.Verbose, 0, e);
						}
						catch (Exception e)
						{
							op.TrySetException(e);
							_console.TraceData(TraceEventType.Error, 0, e);
						}
					}
				}

				if (!_cancellationSource.IsCancellationRequested)
				{
					// Disable all states that are invisible (covered by the top exclusive state).
					//UpdateStateStack();

					// Activate top state if no popups are active.
					ActivateTopState();
				}
			}
			finally
			{
				_stackOperationsProcessor = null;
			}
		}

		private async Task<IAppState> ProcessPushOperation(AppStatePushOperation op)
		{
			Debug.Assert(op != null);

			AppState result = null;

			try
			{
				var stateName = AppState.GetStateName(op.ControllerType);

				// Replace the specified state with the new one.
				if (op.Options.HasFlag(PushOptions.Set))
				{
					_console.TraceInformation("Set " + stateName);

					result = await PushStateInternal(op.OwnerState.Owner, op.ControllerType, op.ControllerArgs, op.Options, op.CancellationToken);

					if (op.Transition != null)
					{
						await op.Transition.PlaySetTransition(op.OwnerState, result, op.CancellationToken);
					}

					await PopStateInternal(op.OwnerState, op.CancellationToken);
				}
				// Remove all states from the stack and push the new one.
				else if (op.Options.HasFlag(PushOptions.Reset))
				{
					_console.TraceInformation("Reset " + stateName);

					await PopAllStates(op.CancellationToken);

					result = await PushStateInternal(null, op.ControllerType, op.ControllerArgs, op.Options, op.CancellationToken);

					if (op.Transition != null)
					{
						await op.Transition.PlayPushTransition(result, op.CancellationToken);
					}
				}
				// Just push the new state onto the stack.
				else
				{
					_console.TraceInformation("Push " + stateName);

					result = await PushStateInternal(op.OwnerState, op.ControllerType, op.ControllerArgs, op.Options, op.CancellationToken);

					if (op.Transition != null)
					{
						if (op.OwnerState != null)
						{
							await op.Transition.PlayPushTransition(op.OwnerState, result, op.CancellationToken);
						}
						else
						{
							await op.Transition.PlayPushTransition(result, op.CancellationToken);
						}
					}
				}
			}
			catch
			{
				result?.Dispose();
				throw;
			}

			return result;
		}

		private async Task ProcessPopOperation(AppStatePopOperation op)
		{
			Debug.Assert(op != null);

			if (op.State != null)
			{
				_console.TraceInformation("Pop " + op.State.Name);

				if (op.Transition != null)
				{
					await op.Transition.PlayPopTransition(op.State, op.CancellationToken);
				}

				await PopStateInternal(op.State, op.CancellationToken);
			}
			else
			{
				_console.TraceInformation("PopAll");

				await PopAllStates(op.CancellationToken);
			}
		}

		private bool CanExecuteOperation(AppStateStackOperation op)
		{
			if (op.Task.IsCanceled)
			{
				return false;
			}

			return true;
		}

		private async Task<AppState> PushStateInternal(IAppState owner, Type controllerType, object controllerArgs, PushOptions options, CancellationToken cancellationToken)
		{
			var state = new AppState(_console, this, owner, controllerType, controllerArgs);
			await state.Push(cancellationToken);
			return state;
		}

		private async Task PopStateInternal(AppState state, CancellationToken cancellationToken)
		{
			// Pop all dependent states (states that was pushed onto the stack by the state).
			foreach (var s in _states.ToArray())
			{
				if (s.Owner == state)
				{
					await s.Pop(cancellationToken);
				}
			}

			// Release the state (and its substates). This will also remove state from the stack.
			await state.Pop(cancellationToken);
		}

		private async Task PopAllStates(CancellationToken cancellationToken)
		{
			foreach (var s in _states.ToArray())
			{
				await s.Pop(cancellationToken);
			}
		}

		private void InvokeOperationInitiated(AppStateStackOperation op)
		{
			// TODO
		}

		private void InvokeOperationComplete(AppStateStackOperation op)
		{
			// TODO
		}

		private void InvokeOperationFailed(AppStateStackOperation op)
		{
			// TODO
		}

		private void InvokeStatePushed(IAppState state)
		{
			// TODO
		}

		private void InvokeStateActivated(IAppState state)
		{
			// TODO
		}

		private void InvokeStateDeactivated(IAppState state)
		{
			// TODO
		}

		private void InvokeStatePopped(IAppState state)
		{
			// TODO
		}

		private string GetFullName()
		{
			if (_parentState != null)
			{
				return _parentState.FullName + '.' + _serviceName;
			}

			return _serviceName;
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

			if (!typeof(IAppStateController).IsAssignableFrom(controllerType))
			{
				throw new ArgumentException($"{controllerType.Name} should implement {typeof(IAppStateController).Name} interface", nameof(controllerType));
			}
		}

		private void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetFullName());
			}
		}

		#endregion
	}
}
