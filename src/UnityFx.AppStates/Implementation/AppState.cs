// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace UnityFx.AppStates
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

		private readonly AppStateService _parentStateManager;
		private readonly IAppStateController _controller;
		private readonly AppState _parentState;
		private readonly IAppState _ownerState;

		private readonly TraceSource _console;
		private readonly AppStateStack _stack;
		private readonly string _name;
		private readonly string _fullName;
		private readonly AppStateFlags _flags;
		private readonly object _controllerArgs;

		private AppStateService _substateManager;
		private IAppStateView _view;

		private AppStateState _state;
		private bool _isActive;
		private bool _isActivated;

		#endregion

		#region interface

		internal bool Enabled => _state == AppStateState.Pushed;

		internal AppState(AppStateService parentStateManager, IAppState owner, Type controllerType, object args)
		{
			Debug.Assert(parentStateManager != null);
			Debug.Assert(controllerType != null);

			_parentStateManager = parentStateManager;
			_parentState = parentStateManager.ParentState;
			_ownerState = owner;
			_controllerArgs = args;
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
			}
			else
			{
				_name = GetStateNameSimple(controllerType);
			}

			_fullName = _parentState?.FullName + '.' + _name ?? _name;
			_controller = parentStateManager.CreateStateController(this, controllerType);
		}

		internal void Activate()
		{
			Debug.Assert(_state == AppStateState.Pushed);

			if (!_isActive && (_parentState == null || _parentState.IsActive))
			{
				_console.TraceEvent(TraceEventType.Verbose, 0, "ActivateState " + _fullName);

				_view?.SetInteractable(true);
				_isActive = true;

				if (_controller is IAppStateEvents ce)
				{
					ce.OnActivate(_isActivated);
				}

				_isActivated = true;
				_substateManager?.TryActivateTopState();
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
					_substateManager?.TryDeactivateTopState();

					if (_controller is IAppStateEvents ce)
					{
						ce.OnDeactivate();
					}
				}
				finally
				{
					_view?.SetInteractable(false);
					_isActive = false;
				}
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

		internal IAppStateView GetPrevView()
		{
			var i = _stack.Count - 1;

			while (i >= 0)
			{
				if (_stack[i] != this)
				{
					--i;
				}
			}

			for (; i >= 0; --i)
			{
				var view = _stack[i]._view;

				if (view != null)
				{
					return view;
				}
			}

			return _parentState?.GetPrevView();
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

		public bool IsActive => _isActive;

		public IAppState ParentState => _parentState;

		public IAppState OwnerState => _ownerState;

		public IReadOnlyCollection<IAppState> ChildStates
		{
			get
			{
				ThrowIfDisposed();
				return this;
			}
		}

		public IAppStateView View
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

		#endregion

		#region IAppStateContext

		public object Args => _controllerArgs;

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
					_substateManager = _parentStateManager.CreateSubstateManager(this, _parentStateManager);
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
					_substateManager?.Dispose();
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
