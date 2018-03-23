// Copyright (c) Alexander Bogarsukov.
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
		private readonly IAppStateView _view;
		private readonly AppState _parentState;
		private readonly IAppState _ownerState;

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

		internal bool Enabled => _state == AppStateState.Pushed;

		internal AppState(AppStateManager parentStateManager, IAppState owner, Type controllerType, PushStateArgs args)
		{
			Debug.Assert(parentStateManager != null);
			Debug.Assert(controllerType != null);

			_parentStateManager = parentStateManager;
			_parentState = parentStateManager.ParentState;
			_ownerState = owner;
			_args = args;
			_console = parentStateManager.TraceSource;
			_stack = parentStateManager.StatesEx;

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

			_view = parentStateManager.Shared.CreateView(this);
			_controller = parentStateManager.Shared.CreateController(this, controllerType);
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

		#region IAppState

		public string Id => _id;

		public AppStateFlags Flags => _flags;

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

		public PushStateArgs CreationArgs => _args;

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

		public IAppState ParentState => _parentState;

		public IAppState OwnerState => _ownerState;

		public IAppStateView View => _view;

		public IAppStateController Controller => _controller;

		public bool IsActive => _isActive;

		public IReadOnlyCollection<IAppState> ChildStates => this;

		#endregion

		#region IAppStateContext

		public object Args => _args;

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

		private void ThrowIfDisposed()
		{
			if (_state == AppStateState.Disposed)
			{
				throw new ObjectDisposedException(_id);
			}
		}

		#endregion
	}
}
