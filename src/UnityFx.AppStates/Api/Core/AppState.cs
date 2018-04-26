﻿// Copyright (c) Alexander Bogarsukov.
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
	/// Enumerates state-related flags.
	/// </summary>
	[Flags]
	public enum AppStateFlags
	{
		/// <summary>
		/// No flags.
		/// </summary>
		None = 0
	}

	/// <summary>
	/// A generic application state.
	/// </summary>
	/// <remarks>
	/// By design an application flow is a sequence of state switches. A state may represent a single screen,
	/// a dialog, a menu or some process without any visual representation. States are supposed to be as independent
	/// as possible. Only one state may be active (i.e. process user input) at time, but unlimited number of states may
	/// exist (be rendered on the screen and execute their code) at the same time.
	/// </remarks>
	/// <seealso href="http://gameprogrammingpatterns.com/state.html"/>
	/// <seealso href="https://en.wikipedia.org/wiki/State_pattern"/>
	public class AppState : IDisposable
	{
		#region data

		private enum AppStateState
		{
			Created,
			Pushed,
			Popped,
			Disposed
		}

		private readonly AppStateManager _parentStateManager;
		private readonly IAppStateController _controller;
		private readonly IAppStateView _view;
		private readonly AppState _parentState;
		private readonly AppState _ownerState;

		private readonly TraceSource _console;
		private readonly AppStateStack _stack;
		private readonly string _id;
		private readonly AppStateFlags _flags;
		private readonly PushStateArgs _args;

		private AppStateManager _substateManager;
		private AppStateState _state;
		private bool _isActive;
		private bool _isActivated;

		#endregion

		#region interface

		/// <summary>
		/// Gets whether the state is enabled.
		/// </summary>
		internal bool Enabled => _state == AppStateState.Pushed;

		/// <summary>
		/// Gets a parent state manager instance.
		/// </summary>
		internal IAppStateManager StateManager => _parentStateManager;

		/// <summary>
		/// Gets a substate manager instance.
		/// </summary>
		internal IAppStateManager SubstateManager
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

		internal AppState(AppStateManager parentStateManager, AppState owner, Type controllerType, PushStateArgs args)
		{
			Debug.Assert(parentStateManager != null);
			Debug.Assert(controllerType != null);

			_parentStateManager = parentStateManager;
			_parentState = parentStateManager.ParentState;
			_ownerState = owner;
			_args = args;
			_console = parentStateManager.TraceSource;
			_stack = parentStateManager.States;

			if (Attribute.GetCustomAttribute(controllerType, typeof(AppStateControllerAttribute)) is AppStateControllerAttribute paramsAttr)
			{
				if (string.IsNullOrEmpty(paramsAttr.Id))
				{
					_id = GetStateNameSimple(controllerType);
				}
				else
				{
					_id = paramsAttr.Id;
				}

				_flags = paramsAttr.Flags;
			}
			else
			{
				_id = GetStateNameSimple(controllerType);
			}

			_view = parentStateManager.Shared.ViewManager.CreateView(_id, GetPrevView());
			_controller = parentStateManager.Shared.ControllerFactory.CreateController(controllerType, this);
		}

		internal IAsyncOperation Push(IAppStateOperationInfo op)
		{
			Debug.Assert(_state == AppStateState.Created);

			_console.TraceData(TraceEventType.Verbose, op.OperationId, "PushState " + _id);
			_stack.Add(this);
			_state = AppStateState.Pushed;
			_substateManager?.SetEnabled();

			return PushInternal();
		}

		internal void Pop(IAppStateOperationInfo op)
		{
			if (_state == AppStateState.Pushed)
			{
				_substateManager?.Pop(op);
				_console.TraceData(TraceEventType.Verbose, op.OperationId, "PopState " + _id);
				_stack.Remove(this);
				_state = AppStateState.Popped;
			}

			Dispose();
		}

		internal void Activate(IAppStateOperationInfo op)
		{
			Debug.Assert(_state == AppStateState.Pushed);

			if (!_isActive && (_parentState == null || _parentState.IsActive))
			{
				_console.TraceEvent(TraceEventType.Verbose, op.OperationId, "ActivateState " + _id);

				_view.Interactable = true;
				_isActive = true;

				ActivateInternal();

				_isActivated = true;
				_substateManager?.TryActivateTopState(op);
			}
		}

		internal void Deactivate(IAppStateOperationInfo op)
		{
			Debug.Assert(_state == AppStateState.Pushed);

			if (_isActive)
			{
				_console.TraceData(TraceEventType.Verbose, op.OperationId, "DeactivateState " + _id);

				try
				{
					_substateManager?.TryDeactivateTopState(op);
					DeactivateInternal();
				}
				finally
				{
					_view.Interactable = false;
					_isActive = false;
				}
			}
		}

		internal void GetStatesRecursive(ICollection<AppState> states)
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
				else
				{
					break;
				}
			}

			if (i >= 0)
			{
				return _stack[i].View;
			}

			return _parentState?.GetPrevView();
		}

		internal static string GetStateName(Type controllerType)
		{
			if (Attribute.GetCustomAttribute(controllerType, typeof(AppStateControllerAttribute)) is AppStateControllerAttribute attr)
			{
				if (string.IsNullOrEmpty(attr.Id))
				{
					return GetStateNameSimple(controllerType);
				}
				else
				{
					return attr.Id;
				}
			}

			return GetStateNameSimple(controllerType);
		}

		internal static string GetStateNameSimple(Type controllerType)
		{
			var name = controllerType.Name;

			if (name.EndsWith("State"))
			{
				name = name.Substring(0, name.Length - 5).ToLowerInvariant();
			}
			else if (name.EndsWith("Controller"))
			{
				name = name.Substring(0, name.Length - 10).ToLowerInvariant();
			}

			return name;
		}

		#endregion

		#region interface

		/// <summary>
		/// Gets the state type identifier.
		/// </summary>
		public string Id => _id;

		/// <summary>
		/// Gets the state flags.
		/// </summary>
		public AppStateFlags Flags => _flags;

		/// <summary>
		/// Gets the state path.
		/// </summary>
		public string Path
		{
			get
			{
				var localPath = '/' + _id;

				if (_parentState == null)
				{
					return _ownerState?.Path + localPath ?? localPath;
				}

				return _parentState.Path;
			}
		}

		/// <summary>
		/// Gets the state creation arguments.
		/// </summary>
		public PushStateArgs CreationArgs => _args;

		/// <summary>
		/// Gets a deeplink representing this state.
		/// </summary>
		public Uri Deeplink
		{
			get
			{
				var uriBuilder = new UriBuilder(_parentStateManager.Shared.DeeplinkScheme, _parentStateManager.Shared.DeeplinkDomain)
				{
					Path = Path
				};

				if (_parentState != null)
				{
					uriBuilder.Fragment = _id;
				}

				return uriBuilder.Uri;
			}
		}

		/// <summary>
		/// Gets a parent state.
		/// </summary>
		public AppState ParentState => _parentState;

		/// <summary>
		/// Gets a state that created this one (or <see langword="null"/>).
		/// </summary>
		public AppState OwnerState => _ownerState;

		/// <summary>
		/// Gets a view instance attached to the state.
		/// </summary>
		public IAppStateView View => _view;

		/// <summary>
		/// Gets a controller attached to the state.
		/// </summary>
		public IAppStateController Controller => _controller;

		/// <summary>
		/// Gets a value indicating whether the state is active.
		/// </summary>
		public bool IsActive => _isActive;

		/// <summary>
		/// Gets a collection of the state's children.
		/// </summary>
		public IReadOnlyCollection<AppState> Substates => _substateManager?.States ?? EmptyCollection<AppState>.Instance;

		/// <summary>
		/// Throws <see cref="ObjectDisposedException"/> if the instance is disposed.
		/// </summary>
		protected void ThrowIfDisposed()
		{
			if (_state == AppStateState.Disposed)
			{
				throw new ObjectDisposedException(_id);
			}
		}

		#endregion

		#region IDisposable

		/// <inheritdoc/>
		public void Dispose()
		{
			if (_state != AppStateState.Disposed)
			{
				_state = AppStateState.Disposed;
				_stack.Remove(this);

				try
				{
					_substateManager?.Dispose();

					if (_controller is IDisposable d)
					{
						d.Dispose();
					}
				}
				finally
				{
					_view.Dispose();
				}
			}
		}

		#endregion

		#region implementation

		private IAsyncOperation PushInternal()
		{
			if (_controller is AppStateController c)
			{
				return c.OnLoadContent();
			}

			return AsyncResult.CompletedOperation;
		}

		private void ActivateInternal()
		{
			if (_controller is AppStateController c)
			{
				c.OnActivate(_isActivated);
			}
		}

		private void DeactivateInternal()
		{
			if (_controller is AppStateController c)
			{
				c.OnDeactivate();
			}
		}

		private void PopInternal()
		{
			// do nothing
		}

		#endregion
	}
}
