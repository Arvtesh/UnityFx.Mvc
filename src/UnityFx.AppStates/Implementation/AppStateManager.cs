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
	/// <summary>
	/// Implementation of <see cref="IAppStateService"/>.
	/// </summary>
	internal sealed class AppStateManager : MonoBehaviour, IAppStateService, IAppStateServiceSettings, IAppStateStack, IAppStateManagerInternal
	{
		#region data

		private const int _maxStackOperationsCount = 16;
		private const string _serviceName = "StateManager";

		private TraceSource _console;

		private List<AppState> _states = new List<AppState>();
		private Queue<AppStateStackOperation> _stackOperations = new Queue<AppStateStackOperation>();

		private IAppViewFactory _viewManager;
		private IAppState _parentState;
		private object _appContext;
		private bool _disposed;

		#endregion

		#region interface

		public void Initialize(IAppViewFactory viewManager, object appContext, SourceLevels traceLevel)
		{
			_console = new TraceSource(_serviceName, traceLevel);
			_viewManager = viewManager;
			_appContext = appContext;
		}

		public void Initialize(IAppState parentState, TraceSource console, IAppViewFactory viewManager, object appContext)
		{
			_console = console;
			_viewManager = viewManager;
			_parentState = parentState;
			_appContext = appContext;
		}

		#endregion

		#region MonoBehaviour

		private void OnEnable()
		{
			
		}

		private void OnDisable()
		{
			
		}

		private void OnDestroy()
		{
		}

		private void Update()
		{
			
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

		public IAppStateStack States => this;

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
			PushState(null, typeof(T), options, args);
		}

		public Task<IAppState> PushStateAsync<T>(PushOptions options, object args) where T : class, IAppStateController
		{
			ThrowIfDisposed();
			return PushStateAsync(null, typeof(T), options, args);
		}

		public void PushState(Type controllerType, PushOptions options, object args)
		{
			ThrowIfDisposed();
			PushState(null, controllerType, options, args);
		}

		public Task<IAppState> PushStateAsync(Type controllerType, PushOptions options, object args)
		{
			ThrowIfDisposed();
			return PushStateAsync(null, controllerType, options, args);
		}

		#endregion

		#region IAppStateManagerInternal

		public object AppContext => _appContext;

		public IAppState ParentState => _parentState;

		public IAppStateManager CreateSubstateManager(IAppState state)
		{
			var result = state.Go.AddComponent<AppStateManager>();
			result.Initialize(state, _console, _viewManager, _appContext);
			return result;
		}

		public IAppView CreateView(IAppState state)
		{
			var exclusive = !state.Flags.HasFlag(AppStateFlags.Popup);
			var insertAfterView = default(IAppView);

			// TODO
			return _viewManager.CreateView(state.Name, exclusive, insertAfterView, state);
		}

		public void PushState(IAppState ownerState, Type controllerType, PushOptions options, object stateArgs)
		{
			ThrowIfDisposed();
			ThrowIfInvalidControllerType(controllerType);

			var op = new AppStateStackOperation(options, ownerState, controllerType, stateArgs);
			AddStackOperation(op);
		}

		public Task<IAppState> PushStateAsync(IAppState ownerState, Type controllerType, PushOptions options, object stateArgs)
		{
			ThrowIfDisposed();
			ThrowIfInvalidControllerType(controllerType);

			var op = new AppStateStackOperation(options, ownerState, controllerType, stateArgs);
			AddStackOperation(op);
			return op.Task;
		}

		public void PopState(IAppState state)
		{
			ThrowIfDisposed();

			var op = new AppStateStackOperation(state);
			AddStackOperation(op);
		}

		public Task PopStateAsync(IAppState state)
		{
			ThrowIfDisposed();

			var op = new AppStateStackOperation(state);
			AddStackOperation(op);
			return op.Task;
		}

		public void ReleaseState(IAppState state)
		{
			// TODO
		}

		#endregion

		#region IAppStateStack

		public IAppState Peek()
		{
			var n = _states.Count;

			if (n > 0)
			{
				return _states[n - 1];
			}

			return null;
		}

		#endregion

		#region IReadOnlyCollection

		public int Count => _states.Count;

		#endregion

		#region IEnumerable

		public IEnumerator<IAppState> GetEnumerator() => _states.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _states.GetEnumerator();

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
