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

		private object _appContext;
		private object _stateArgs;
		private string _name;

		private bool _disposed;
		private bool _isActive;
		private bool _isActivated;

		#endregion

		#region interface

		public void Initialize(IAppStateManagerInternal parentStateManager, IAppState owner, object args)
		{
			_parentStateManager = parentStateManager;
			_parentState = _parentStateManager.ParentState;
			_ownerState = owner;
			_appContext = _parentStateManager.Context;
			_name = string.Empty;
			_stateArgs = args;
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
			_view?.Dispose();
		}

		#endregion

		#region IAppState

		public IReadOnlyCollection<IAppState> Children => this;

		public IAppStateController Controller => _controller;

		#endregion

		#region IAppStateContext

		public object AppContext => _appContext;

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

		#region IAppStateInfo

		public GameObject Go => gameObject;

		public Bounds Bounds => _view?.Bounds ?? new Bounds(transform.position, Vector3.zero);

		public string Name => _name;

		public string FullName => _parentState?.FullName + '.' + _name ?? _name;

		public object Args => _stateArgs;

		public bool IsActive => _isActive;

		public IAppState Parent => _parentState;

		public IAppState Owner => _ownerState;

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

		#endregion

		#region IAppStateStackController

		public void PushState<T>(PushOptions options, object args) where T : class, IAppStateController
		{
			ThrowIfDisposed();
			_parentStateManager.PushState(this, typeof(T), options, args);
		}

		public Task<IAppState> PushStateAsync<T>(PushOptions options, object args) where T : class, IAppStateController
		{
			ThrowIfDisposed();
			return _parentStateManager.PushStateAsync(this, typeof(T), options, args);
		}

		public void PushState(Type controllerType, PushOptions options, object args)
		{
			ThrowIfDisposed();
			_parentStateManager.PushState(this, controllerType, options, args);
		}

		public Task<IAppState> PushStateAsync(Type controllerType, PushOptions options, object args)
		{
			ThrowIfDisposed();
			return _parentStateManager.PushStateAsync(this, controllerType, options, args);
		}

		public void PopState()
		{
			ThrowIfDisposed();
			_parentStateManager.PopState(this);
		}

		public Task PopStateAsync()
		{
			ThrowIfDisposed();
			return _parentStateManager.PopStateAsync(this);
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
				Destroy(gameObject);
				GC.SuppressFinalize(this);
			}
		}

		#endregion

		#region implementation

		private void ThrowIfDisposed()
		{
			if (_disposed || !this)
			{
				throw new ObjectDisposedException(_name);
			}
		}

		#endregion
	}
}
