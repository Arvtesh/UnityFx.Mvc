// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UnityFx.App
{
	/// <summary>
	/// Enumerates states of <see cref="AppState"/>.
	/// </summary>
	internal enum AppStateState
	{
		Created,
		Pushed,
		Popped,
		Disposed
	}

	/// <summary>
	/// Implementation of <see cref="IAppState"/>.
	/// </summary>
	internal sealed class AppState : IAppState, IAppStateContext, IReadOnlyCollection<IAppState>, IDisposable
	{
		#region data

		private readonly AppStateManager _parentStateManager;
		private readonly IAppStateController _controller;
		private readonly IAppStateEvents _controllerEvents;
		private readonly IAppState _parentState;
		private readonly IAppState _ownerState;

		private readonly TraceSource _console;
		private readonly AppStateStack _stack;
		private readonly string _name;
		private readonly string _fullName;
		private readonly AppStateFlags _flags;
		private readonly int _layer;
		private readonly object _stateArgs;
		private readonly AppStateEventArgs _eventArgs;

		private AppStateManager _substateManager;
		private IAppView _view;

		private AppStateState _state;
		private bool _isActive;
		private bool _isActivated;

		#endregion

		#region interface

		internal AppState(AppStateManager parentStateManager, IAppState owner, Type controllerType, object args)
		{
			Debug.Assert(parentStateManager != null);
			Debug.Assert(controllerType != null);

			_parentStateManager = parentStateManager;
			_parentState = parentStateManager.ParentState;
			_ownerState = owner;
			_stateArgs = args;
			_eventArgs = new AppStateEventArgs(this);
			_console = parentStateManager.TraceSource;
			_stack = parentStateManager.StatesEx;

			if (Attribute.GetCustomAttribute(controllerType, typeof(AppStateControllerAttribute)) is AppStateControllerAttribute paramsAttr)
			{
				if (string.IsNullOrEmpty(paramsAttr.Name))
				{
					_name = GetStateNameSimple(controllerType);
				}
				else
				{
					_name = paramsAttr.Name;
				}

				_flags = paramsAttr.Flags;
				_layer = paramsAttr.Layer;
			}
			else
			{
				_name = GetStateNameSimple(controllerType);
			}

			// Force AppStateFlags.Popup flag for child states.
			if (_parentState != null)
			{
				_flags |= AppStateFlags.Popup;
			}

			_fullName = _parentState?.FullName + '.' + _name ?? _name;
			_controller = parentStateManager.CreateStateController(this, controllerType);
			_controllerEvents = _controller as IAppStateEvents;
		}

		internal void Activate()
		{
			Debug.Assert(_state == AppStateState.Pushed);

			if (!_isActive)
			{
				_console.TraceEvent(TraceEventType.Verbose, 0, "ActivateState " + _fullName);

				if (_view != null)
				{
					_view.Interactable = true;
				}

				_isActive = true;
				_controllerEvents?.OnActivate(!_isActivated);
				_isActivated = true;
				_parentStateManager.InvokeStateActivated(_eventArgs);
				_substateManager?.ActivateTopState();
			}
		}

		internal void Deactivate()
		{
			Debug.Assert(_state == AppStateState.Pushed);

			if (_isActive)
			{
				_console.TraceData(TraceEventType.Verbose, 0, "DeactivateState " + _fullName);

				try
				{
					_substateManager?.DeactivateTopState();
					_controllerEvents?.OnDeactivate();
					_parentStateManager.InvokeStateDeactivated(_eventArgs);
				}
				finally
				{
					if (_view != null)
					{
						_view.Interactable = false;
					}

					_isActive = false;
				}
			}
		}

		internal async Task Push(CancellationToken cancellationToken)
		{
			Debug.Assert(_state == AppStateState.Created);

			// Push the state onto the stack.
			_state = AppStateState.Pushed;
			_console.TraceData(TraceEventType.Verbose, 0, "- PushState " + _fullName);
			_stack.Add(this);
			_controllerEvents?.OnPush();
			_parentStateManager.InvokeStatePushed(_eventArgs);

			// Enable substates processing.
			_substateManager?.SetEnabled();

			// Load state content.
			if (_controller is IAppStateContent sc)
			{
				await sc.LoadContent(cancellationToken);
			}
		}

		internal async Task Pop(CancellationToken cancellationToken)
		{
			Debug.Assert(_state == AppStateState.Pushed);

			try
			{
				// Stop any pending substate operations.
				if (_substateManager != null)
				{
					await _substateManager.PopAll();
				}

				// Pop the state from the state stack (actually the state is removed from the stack in Dispose call).
				_state = AppStateState.Popped;
				_console.TraceData(TraceEventType.Verbose, 0, "- PopState " + _fullName);
				_controllerEvents?.OnPop();
				_parentStateManager.InvokeStatePopped(_eventArgs);
			}
			catch (Exception e)
			{
				_console.TraceData(TraceEventType.Error, 0, e);
			}
			finally
			{
				Dispose();
			}
		}

		internal void GetStatesRecursive(ICollection<IAppState> states)
		{
			Debug.Assert(_state != AppStateState.Disposed);

			if (_substateManager != null)
			{
				_substateManager.GetStatesRecursive(states);
			}
		}

		internal static string GetStateName(Type controllerType)
		{
			if (Attribute.GetCustomAttribute(controllerType, typeof(AppStateControllerAttribute)) is AppStateControllerAttribute attr)
			{
				if (string.IsNullOrEmpty(attr.Name))
				{
					return GetStateNameSimple(controllerType);
				}
				else
				{
					return attr.Name;
				}
			}

			return GetStateNameSimple(controllerType);
		}

		internal static string GetStateNameSimple(Type controllerType)
		{
			var name = controllerType.Name;

			if (name.EndsWith("State"))
			{
				name = name.Substring(0, name.Length - 5);
			}
			else if (name.EndsWith("Controller"))
			{
				name = name.Substring(0, name.Length - 10);
			}

			return name;
		}

		#endregion

		#region IAppState

		public string Name => _name;

		public string FullName => _fullName;

		public AppStateFlags Flags => _flags;

		public int Layer => _layer;

		public object Args => _stateArgs;

		public bool IsActive => _isActive;

		public IAppState Parent => _parentState;

		public IAppState Owner => _ownerState;

		public IReadOnlyCollection<IAppState> ChildStates
		{
			get
			{
				ThrowIfDisposed();
				return this;
			}
		}

		public IAppView View
		{
			get
			{
				ThrowIfDisposed();

				if (_view == null)
				{
					_view = _parentStateManager.CreateView(this);
				}

				return _view;
			}
		}

		public IAppStateController Controller
		{
			get
			{
				ThrowIfDisposed();
				return _controller;
			}
		}

		public Task CloseAsync()
		{
			ThrowIfDisposed();
			return _parentStateManager.PopState(this);
		}

		#endregion

		#region IAppStateContext

		public IAppState State
		{
			get
			{
				ThrowIfDisposed();
				return this;
			}
		}

		public IAppStateManager StateManager
		{
			get
			{
				ThrowIfDisposed();
				return _parentStateManager;
			}
		}

		public IAppStateManager SubstateManager
		{
			get
			{
				ThrowIfDisposed();

				if (_substateManager == null)
				{
					_substateManager = _parentStateManager.CreateSubstateManager(this, _parentStateManager, _state == AppStateState.Pushed);
				}

				return _substateManager;
			}
		}

		#endregion

		#region IReadOnlyCollection

		public int Count => _substateManager?.States.Count ?? 0;

		#endregion

		#region IEnumerable

		public IEnumerator<IAppState> GetEnumerator()
		{
			ThrowIfDisposed();
			return _substateManager?.States.GetEnumerator() ?? Enumerable.Empty<IAppState>().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (this as IEnumerable<IAppState>).GetEnumerator();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (_state != AppStateState.Disposed)
			{
				_state = AppStateState.Disposed;
				_stack.Remove(this);

				try
				{
					if (_controller is IDisposable d)
					{
						d.Dispose();
					}
				}
				finally
				{
					_view?.Dispose();
				}
			}
		}

		#endregion

		#region implementation

		private void ThrowIfDisposed()
		{
			if (_state == AppStateState.Disposed)
			{
				throw new ObjectDisposedException(_name);
			}
		}

		#endregion
	}
}
