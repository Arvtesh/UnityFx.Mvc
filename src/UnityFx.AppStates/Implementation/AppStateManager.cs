// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
		private IEnumerator _worker;

		private IAppViewFactory _viewManager;
		private IAppStateInternal _parentState;
		private object _appContext;
		private bool _disposed;

		#endregion

		#region interface

		public void Initialize(IAppViewFactory viewManager, object appContext)
		{
			_console = new TraceSource(_serviceName);
			_states = new AppStateStack(transform);
			_stackOperations = new Queue<AppStateStackOperation>();
			_viewManager = viewManager;
			_appContext = appContext;
		}

		public void Initialize(IAppStateInternal parentState, TraceSource console, IAppViewFactory viewManager, object appContext)
		{
			_console = console;
			_states = new AppStateStack(transform);
			_stackOperations = new Queue<AppStateStackOperation>();
			_viewManager = viewManager;
			_parentState = parentState;
			_appContext = appContext;
		}

		#endregion

		#region MonoBehaviour

		private void OnEnable()
		{
			if (_worker != null)
			{
				// Resume the coroutine stopped when the behavious was disabled.
				StartCoroutine(_worker);
			}
			else
			{
				ActivateTopState();
			}
		}

		private void OnDisable()
		{
			DeactivateTopState();
		}

		private void OnDestroy()
		{
		}

		private void Update()
		{
			// Start worker coroutine if there are any operatinos in the stack.
			if (_worker == null && _stackOperations.Count > 0)
			{
				_worker = ProcessStackOperations(true);
				StartCoroutine(_worker);
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
			return PushStateAsync(_parentState, typeof(T), options, args);
		}

		public void PushState(Type controllerType, PushOptions options, object args)
		{
			ThrowIfDisposed();
			PushState(_parentState, controllerType, options, args);
		}

		public Task<IAppState> PushStateAsync(Type controllerType, PushOptions options, object args)
		{
			ThrowIfDisposed();
			return PushStateAsync(_parentState, controllerType, options, args);
		}

		#endregion

		#region IAppStateManagerInternal

		public object AppContext => _appContext;

		public IAppStateInternal ParentState => _parentState;

		public IAppStateManagerInternal CreateSubstateManager(IAppStateInternal state)
		{
			ThrowIfDisposed();

			var result = state.Go.AddComponent<AppStateManager>();
			result.Initialize(state, _console, _viewManager, _appContext);
			return result;
		}

		public IAppView CreateView(IAppStateInternal state)
		{
			ThrowIfDisposed();

			var exclusive = !state.Flags.HasFlag(AppStateFlags.Popup);
			var insertAfterView = default(IAppView);

			// TODO
			return _viewManager.CreateView(state.Name, exclusive, insertAfterView, state);
		}

		public void PushState(IAppStateInternal ownerState, Type controllerType, PushOptions options, object stateArgs)
		{
			ThrowIfDisposed();
			ThrowIfInvalidControllerType(controllerType);

			var op = new AppStateStackOperation(options, ownerState, controllerType, stateArgs);
			AddStackOperation(op);
		}

		public Task<IAppState> PushStateAsync(IAppStateInternal ownerState, Type controllerType, PushOptions options, object stateArgs)
		{
			ThrowIfDisposed();
			ThrowIfInvalidControllerType(controllerType);

			var op = new AppStateStackOperation(options, ownerState, controllerType, stateArgs);
			AddStackOperation(op);
			return op.Task;
		}

		public void PopState(IAppStateInternal state)
		{
			ThrowIfDisposed();

			var op = new AppStateStackOperation(state);
			AddStackOperation(op);
		}

		public Task PopStateAsync(IAppStateInternal state)
		{
			ThrowIfDisposed();

			var op = new AppStateStackOperation(state);
			AddStackOperation(op);
			return op.Task;
		}

		public void PopAll()
		{
			ThrowIfDisposed();

			if (_states.TryPeekEx(out var topState))
			{
				topState.Deactivate();

				foreach (var state in _states.ToArray())
				{
					state.Pop();
					state.Dispose();
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

		private IEnumerator ProcessStackOperations(bool calledFromUpdate)
		{
			if (calledFromUpdate)
			{
				yield return new WaitForEndOfFrame();
			}

			if (_stackOperations.Count > 0)
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

						if (op.Operation == StackOperation.Push)
						{
							
						}
						else if (op.Operation == StackOperation.Pop)
						{
							// TODO: Play transition animation.
							PopStateInternal(op.State, op);
						}
					}
				}

				// Disable all states that are invisible (covered by the top exclusive state).
				//UpdateStateStack();

				// Activate top state if no popups are active.
				if (calledFromUpdate)
				{
					ActivateTopState();
				}

				// Reset the coroutine reference.
				_worker = null;
			}
		}

		private bool CanExecuteOperation(AppStateStackOperation op)
		{
			// TODO
			return true;
		}

		private void PopDependentStates(IAppStateInternal state)
		{
			foreach (var s in _states.ToArray())
			{
				if (s.Owner == state)
				{
					state.Pop();
					state.Dispose();
				}
			}
		}

		private void PopStateInternal(IAppStateInternal state, TaskCompletionSource<IAppState> tcs)
		{
			// 1) Pop all dependent states (states that was pushed onto the stack by the state).
			PopDependentStates(state);

			// 2) Release the state (and its substates). This will also remove state from the stack.
			state.Pop();
			state.Dispose();

			// 3) Set operation result.
			tcs.SetResult(null);
		}

		private void ActivateTopState()
		{
			if (_states.TryPeekEx(out var state))
			{
				if (state.Activate())
				{
					InvokeStateActivated(state);
				}
			}
		}

		private void DeactivateTopState()
		{
			if (_states.TryPeekEx(out var state))
			{
				if (state.Deactivate())
				{
					InvokeStateDeactivated(state);
				}
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
