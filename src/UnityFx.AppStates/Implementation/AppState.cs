// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.App
{
	/// <summary>
	/// Implementation of <see cref="IAppState"/>.
	/// </summary>
	internal sealed class AppState : MonoBehaviour, IAppState, IAppStateContext, IReadOnlyCollection<IAppState>
	{
		#region data

		private IAppStateManagerInternal _parentStateManager;
		private IAppStateManager _substateManager;
		private IAppStateController _controller;
		private IAppState _parentState;
		private IAppState _ownerState;
		private IAppView _view;

		private string _name;
		private AppStateFlags _flags;
		private int _layer;
		private object _stateArgs;

		private bool _isActive;
		private bool _isActivated;
		private bool _disposed;

		#endregion

		#region interface

		public void Initialize(IAppStateManagerInternal parentStateManager, IAppState owner, object args, IAppStateController controller)
		{
			_parentStateManager = parentStateManager;
			_controller = controller;
			_parentState = _parentStateManager.ParentState;
			_ownerState = owner;
			_stateArgs = args;

			InitStateParams();
		}

		public void GetStatesRecursive(ICollection<IAppState> states)
		{
			if (_substateManager != null)
			{
				_substateManager.GetStatesRecursive(states);
			}
		}

		#endregion

		#region MonoBehaviour

		private void OnDestroy()
		{
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

		#endregion

		#region IAppState

		public GameObject Go => gameObject;

		public Bounds Bounds => _view?.Bounds ?? new Bounds(transform.position, Vector3.zero);

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
			return _parentStateManager.PopStateAsync(this);
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
			if (!_disposed && this)
			{
				_disposed = true;
				_parentStateManager.ReleaseState(this);
				Destroy(gameObject);
				GC.SuppressFinalize(this);
			}
		}

		#endregion

		#region implementation

		private void InitStateParams()
		{
			if (Attribute.GetCustomAttribute(_controller.GetType(), typeof(AppStateParamsAttribute)) is AppStateParamsAttribute paramsAttr)
			{
				if (string.IsNullOrEmpty(paramsAttr.Name))
				{
					_name = GenerateStateName(_controller);
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
				_name = GenerateStateName(_controller);
				_flags = AppStateFlags.Popup;
			}
			else
			{
				_name = GenerateStateName(_controller);
			}
		}

		private void ThrowIfDisposed()
		{
			if (_disposed || !this)
			{
				throw new ObjectDisposedException(_name);
			}
		}

		private static string GenerateStateName(object obj)
		{
			var name = obj.GetType().Name;

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
