// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.App
{
	using Debug = System.Diagnostics.Debug;

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

		private readonly IAppStateManagerInternal _parentStateManager;
		private readonly IAppStateController _controller;
		private readonly IAppStateEvents _controllerEvents;
		private readonly IAppState _parentState;
		private readonly IAppState _ownerState;

		private readonly TraceSource _console;
		private readonly AppStateStack _stack;
		private readonly GameObject _go;
		private readonly string _name;
		private readonly AppStateFlags _flags;
		private readonly int _layer;
		private readonly object _stateArgs;

		private IAppStateTransition _transition;
		private IAppStateManagerInternal _substateManager;
		private IAppView _view;

		private AppStateState _state;
		private bool _isActive;
		private bool _isActivated;

		#endregion

		#region interface

		internal IAppStateTransition Transition => _transition;

		internal AppState(GameObject go, TraceSource console, IAppStateManagerInternal parentStateManager, IAppState owner, Type controllerType, object args)
		{
			Debug.Assert(console != null);
			Debug.Assert(parentStateManager != null);
			Debug.Assert(controllerType != null);

			_parentStateManager = parentStateManager;
			_parentState = _parentStateManager.ParentState;
			_ownerState = owner;
			_stateArgs = args;
			_console = console;
			_stack = _parentStateManager.StatesEx;
			_go = go;

			if (Attribute.GetCustomAttribute(controllerType, typeof(AppStateParamsAttribute)) is AppStateParamsAttribute paramsAttr)
			{
				if (string.IsNullOrEmpty(paramsAttr.Name))
				{
					_name = GenerateStateName(controllerType);
				}
				else
				{
					_name = paramsAttr.Name;
				}

				_flags = paramsAttr.Flags;
				_layer = paramsAttr.Layer;
			}
			else if (_parentState != null)
			{
				_name = GenerateStateName(controllerType);
				_flags = AppStateFlags.Popup;
			}
			else
			{
				_name = GenerateStateName(controllerType);
			}

			if (controllerType.IsSubclassOf(typeof(Component)))
			{
				if (_go)
				{
					_controller = _go.AddComponent(controllerType) as IAppStateController;
				}
				else
				{
					throw new ArgumentNullException(nameof(go));
				}
			}
			else
			{
				_controller = Activator.CreateInstance(controllerType) as IAppStateController;
			}

			_controllerEvents = _controller as IAppStateEvents;

			try
			{
				_controllerEvents?.OnInitialize(this);
			}
			catch (Exception e)
			{
				_console.TraceData(TraceEventType.Error, 0, e);
			}
		}

		internal bool Activate()
		{
			Debug.Assert(_state == AppStateState.Pushed);

			if (!_isActive)
			{
				_console.TraceEvent(TraceEventType.Verbose, 0, "ActivateState " + _name);

				if (_view != null)
				{
					_view.Interactable = true;
				}

				_isActive = true;

				try
				{
					_controllerEvents?.OnActivate(!_isActivated);
				}
				catch (Exception e)
				{
					_console.TraceData(TraceEventType.Error, 0, e);
				}

				_isActivated = true;
				_substateManager?.ActivateTopState();
				return true;
			}

			return false;
		}

		internal bool Deactivate()
		{
			Debug.Assert(_state == AppStateState.Pushed);

			if (_isActive)
			{
				_console.TraceData(TraceEventType.Verbose, 0, "DeactivateState " + _name);
				_substateManager?.DeactivateTopState();

				try
				{
					_controllerEvents?.OnDeactivate();
					return true;
				}
				catch (Exception e)
				{
					_console.TraceData(TraceEventType.Error, 0, e);
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

			return false;
		}

		internal async Task Push(CancellationToken cancellationToken)
		{
			Debug.Assert(_state == AppStateState.Created);

			_state = AppStateState.Pushed;
			_console.TraceData(TraceEventType.Verbose, 0, "PushState " + _name);
			_stack.Add(this);

			try
			{
				_controllerEvents?.OnPush();
			}
			catch (Exception e)
			{
				_console.TraceData(TraceEventType.Error, 0, e);
			}

			// Load state content if any.
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
				_state = AppStateState.Popped;
				_console.TraceData(TraceEventType.Verbose, 0, "PopState " + _name);

				if (_substateManager != null)
				{
					await _substateManager.PopAll();
				}

				try
				{
					_controllerEvents?.OnPop();
				}
				catch (Exception e)
				{
					_console.TraceData(TraceEventType.Error, 0, e);
				}
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

		#endregion

		#region IAppState

		public GameObject Go => _go;

		public Bounds Bounds => View.Bounds;

		public string Name => _name;

		public string FullName => _parentState?.FullName + '.' + _name ?? _name;

		public AppStateFlags Flags => _flags;

		public int Layer => _layer;

		public object Args => _stateArgs;

		public bool IsActive => _isActive;

		public IAppState Parent => _parentState;

		public IAppState Owner => _ownerState;

		public IReadOnlyCollection<IAppState> ChildStates => this;

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

		public IAppStateController Controller => _controller;

		public void Close()
		{
			ThrowIfDisposed();
			_parentStateManager.PopState(this);
		}

		public Task CloseAsync()
		{
			ThrowIfDisposed();
			return _parentStateManager.PopState(this);
		}

		#endregion

		#region IAppStateContext

		public object AppContext => _parentStateManager.AppContext;

		public IAppState State => this;

		public IAppStateManager StateManager => _parentStateManager;

		public IAppStateManager SubstateManager
		{
			get
			{
				ThrowIfDisposed();

				if (_substateManager == null)
				{
					_substateManager = _parentStateManager.CreateSubstateManager(this);
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
					if (_go)
					{
						GameObject.Destroy(_go);
					}

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

		private static string GenerateStateName(Type controllerType)
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
	}
}
