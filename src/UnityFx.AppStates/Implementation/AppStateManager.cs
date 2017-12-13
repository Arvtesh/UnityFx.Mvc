// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.App
{
	using Debug = System.Diagnostics.Debug;

	/// <summary>
	/// Implementation of <see cref="IAppStateService"/>.
	/// </summary>
	internal sealed class AppStateManager : MonoBehaviour, IAppStateService, IAppStateServiceSettings, IAppStateManagerInternal
	{
		#region data

		private const int _maxStackOperationsCount = 32;
		private const string _serviceName = "StateManager";

		private TraceSource _console;

		private AppStateStack _states;
		private Queue<AppStateStackOperation> _stackOperations;
		private CancellationTokenSource _cancellationSource;
		private Task _stackOperationsProcessor;

		private IAppViewFactory _viewManager;
		private AppState _parentState;
		private object _appContext;
		private bool _disposed;

		#endregion

		#region interface

		public void Initialize(IAppViewFactory viewManager, object appContext)
		{
			_console = new TraceSource(_serviceName);
			_states = new AppStateStack();
			_stackOperations = new Queue<AppStateStackOperation>();
			_cancellationSource = new CancellationTokenSource();
			_viewManager = viewManager;
			_appContext = appContext;
		}

		public void Initialize(AppState parentState, TraceSource console, IAppViewFactory viewManager, object appContext)
		{
			_console = console;
			_states = new AppStateStack();
			_stackOperations = new Queue<AppStateStackOperation>();
			_cancellationSource = new CancellationTokenSource();
			_viewManager = viewManager;
			_parentState = parentState;
			_appContext = appContext;
		}

		#endregion

		#region MonoBehaviour

		private void OnDestroy()
		{
		}

		private void LateUpdate()
		{
			if (_stackOperationsProcessor == null && _stackOperations.Count > 0 && !_cancellationSource.IsCancellationRequested)
			{
				_stackOperationsProcessor = ProcessStackOperations();
			}
		}

		#endregion

		#region IAppStateService

		public IAppStateServiceSettings Settings => this;

		#endregion

		#region IAppStateServiceSettings

		public SourceSwitch TraceSwitch { get => _console.Switch; set => _console.Switch = value; }

		public TraceListenerCollection TraceListeners => _console.Listeners;

		#endregion

		#region IAppStateManager

		public IAppStateStack States => _states;

		public IEnumerable<IAppState> GetStatesRecursive()
		{
			var list = new List<IAppState>();
			GetStatesRecursive(list);
			return list;
		}

		public void GetStatesRecursive(ICollection<IAppState> states)
		{
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

		public void PushState<T>(PushOptions options, object args) where T : class, IAppStateController
		{
			ThrowIfDisposed();
			PushState(_parentState, typeof(T), options, args);
		}

		public Task<IAppState> PushStateAsync<T>(PushOptions options, object args) where T : class, IAppStateController
		{
			ThrowIfDisposed();
			return PushState(_parentState, typeof(T), options, args);
		}

		public void PushState(Type controllerType, PushOptions options, object args)
		{
			ThrowIfDisposed();
			PushState(_parentState, controllerType, options, args);
		}

		public Task<IAppState> PushStateAsync(Type controllerType, PushOptions options, object args)
		{
			ThrowIfDisposed();
			return PushState(_parentState, controllerType, options, args);
		}

		#endregion

		#region IAppStateManagerInternal

		public object AppContext => _appContext;

		public AppStateStack StatesEx => _states;

		public AppState ParentState => _parentState;

		public IAppStateManagerInternal CreateSubstateManager(AppState state)
		{
			ThrowIfDisposed();

			var result = state.Go.AddComponent<AppStateManager>();
			result.Initialize(state, _console, _viewManager, _appContext);
			return result;
		}

		public IAppView CreateView(AppState state)
		{
			ThrowIfDisposed();

			var exclusive = !state.Flags.HasFlag(AppStateFlags.Popup);
			var insertAfterView = default(IAppView);

			// TODO
			return _viewManager.CreateView(state.Name, exclusive, insertAfterView, state);
		}

		public Task<IAppState> PushState(AppState ownerState, Type controllerType, PushOptions options, object stateArgs)
		{
			ThrowIfDisposed();
			ThrowIfInvalidControllerType(controllerType);

			var op = new AppStatePushOperation(options, ownerState, null, _cancellationSource.Token, controllerType, stateArgs);
			AddStackOperation(op);
			return op.Task;
		}

		public Task PopState(AppState state)
		{
			ThrowIfDisposed();

			var op = new AppStatePopOperation(state, null, _cancellationSource.Token);
			AddStackOperation(op);
			return op.Task;
		}

		public async Task PopAll()
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

		public void ActivateTopState()
		{
			if (_states.TryPeek(out var state))
			{
				if (state.Activate())
				{
					InvokeStateActivated(state);
				}
			}
		}

		public void DeactivateTopState()
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

		#region IDisposable

		public void Dispose()
		{
			if (!_disposed && this)
			{
				_disposed = true;
				Destroy(gameObject);
				GC.SuppressFinalize(this);
			}
		}

		#endregion

		#region implementation

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

		private async Task ProcessStackOperations()
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
					catch (OperationCanceledException)
					{
						op.SetCanceled();
					}
					catch (Exception e)
					{
						op.SetException(e);
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

			// Reset the task reference.
			_stackOperationsProcessor = null;
		}

		private async Task<IAppState> ProcessPushOperation(AppStatePushOperation op)
		{
			Debug.Assert(op != null);

			AppState result = null;

			try
			{
				// Replace the specified state with the new one.
				if (op.Options.HasFlag(PushOptions.Set))
				{
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
				if (op.Transition != null)
				{
					await op.Transition.PlayPopTransition(op.State, op.CancellationToken);
				}

				await PopStateInternal(op.State, op.CancellationToken);
			}
			else
			{
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
			var stateGo = new GameObject(string.Empty);
			stateGo.transform.SetParent(transform, false);
			stateGo.tag = "GameController";

			var state = new AppState(stateGo, _console, this, owner, controllerType, controllerArgs);
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
			if (_disposed || !this)
			{
				throw new ObjectDisposedException(GetFullName());
			}
		}

		#endregion
	}
}
