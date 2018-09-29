// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityFx.AppStates.Common;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Implementation of <see cref="IAppState"/>.
	/// </summary>
	internal sealed class AppState : TreeListNode<IAppState>, IAppState, IPresentable, IPresentableEvents
	{
		#region data

		private static int _idCounter;

		private readonly AppStateService _stateManager;
		private readonly ViewControllerProxy _controllerProxy;
		private readonly string _id;
		private readonly string _deeplinkId;

		private IAsyncOperation _dismissOp;
		private bool _active;
		private bool _disposed;

		#endregion

		#region interface

		internal AppState(AppStateService stateManager, AppState parentState, Type controllerType, PresentArgs args)
			: base(parentState)
		{
			Debug.Assert(stateManager != null);
			Debug.Assert(controllerType != null);

			_id = Utility.GetNextId("_state", ref _idCounter);
			_deeplinkId = Utility.GetControllerTypeId(controllerType);
			_stateManager = stateManager;
			_stateManager.AddState(this);

			// Controller should be created after the state has been initialized.
			try
			{
				_controllerProxy = new ViewControllerProxy(stateManager.ServiceProvider, this, null, controllerType, args);
			}
			catch
			{
				_stateManager.RemoveState(this);
				throw;
			}
		}

		internal void DismissChildStates()
		{
			var childStates = GetChildStates();

			if (childStates != null)
			{
				foreach (var state in childStates)
				{
					state.OnDismiss();
					state.Dispose();
				}
			}
		}

		internal void SetActive(bool newActive)
		{
			_active = newActive;
		}

		#endregion

		#region IAppState

		public string Id => _id;
		public bool IsActive => _active;
		public IViewController Controller => _controllerProxy.Controller;

		#endregion

		#region IPresenter

		public IAsyncOperation<IViewController> PresentAsync(Type controllerType)
		{
			ThrowIfDisposed();
			return _stateManager.PresentAsync(this, controllerType, PresentArgs.Default);
		}

		public IAsyncOperation<IViewController> PresentAsync(Type controllerType, PresentArgs args)
		{
			ThrowIfDisposed();
			return _stateManager.PresentAsync(this, controllerType, args);
		}

		public IAsyncOperation<TController> PresentAsync<TController>() where TController : class, IViewController
		{
			ThrowIfDisposed();
			return _stateManager.PresentAsync<TController>(this, PresentArgs.Default);
		}

		public IAsyncOperation<TController> PresentAsync<TController>(PresentArgs args) where TController : class, IViewController
		{
			ThrowIfDisposed();
			return _stateManager.PresentAsync<TController>(this, args);
		}

		#endregion

		#region IPresentable

		public IAsyncOperation PresentAsync(IPresentContext presentContext)
		{
			Debug.Assert(presentContext == null);
			Debug.Assert(!_disposed);
			Debug.Assert(!_active);

			return _controllerProxy.PresentAsync(presentContext);
		}

		public IAsyncOperation DismissAsync(IDismissContext dismissContext)
		{
			Debug.Assert(dismissContext == null);
			Debug.Assert(!_disposed);
			Debug.Assert(!_active);

			DismissChildStates();
			OnDismiss();

			return _controllerProxy.DismissAsync(dismissContext);
		}

		#endregion

		#region IPresentableEvents

		public void OnPresent()
		{
			Debug.Assert(!_disposed);
			Debug.Assert(!_active);

			_stateManager.TraceEvent(TraceEventType.Verbose, "Present " + Id);
			_controllerProxy.OnPresent();
		}

		public void OnActivate()
		{
			Debug.Assert(!_disposed);
			Debug.Assert(!_active);

			_stateManager.TraceEvent(TraceEventType.Verbose, "Activate " + Id);
			_controllerProxy.OnActivate();
		}

		public void OnDeactivate()
		{
			Debug.Assert(!_disposed);
			Debug.Assert(_active);

			_stateManager.TraceEvent(TraceEventType.Verbose, "Deactivate " + Id);
			_controllerProxy.OnDeactivate();
		}

		public void OnDismiss()
		{
			Debug.Assert(!_disposed);
			Debug.Assert(!_active);

			_stateManager.TraceEvent(TraceEventType.Verbose, "Dismiss " + Id);
			_controllerProxy.OnDismiss();
		}

		#endregion

		#region IDeeplinkable

		public string DeeplinkId => _deeplinkId;

		#endregion

		#region IDismissable

		public IAsyncOperation DismissAsync()
		{
			if (_dismissOp == null)
			{
				if (_disposed)
				{
					_dismissOp = AsyncResult.CompletedOperation;
				}
				else
				{
					_dismissOp = _stateManager.DismissAsync(this);
				}
			}

			return _dismissOp;
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;

				try
				{
					_controllerProxy.Dispose();
				}
				finally
				{
					_stateManager.RemoveState(this);
				}
			}
		}

		#endregion

		#region implementation

		private Stack<AppState> GetChildStates()
		{
			var result = default(Stack<AppState>);
			var nextState = Next;

			while (nextState != null)
			{
				if (nextState.Parent == this)
				{
					if (result == null)
					{
						result = new Stack<AppState>();
					}

					result.Push((AppState)nextState);
				}

				nextState = nextState.Next;
			}

			return result;
		}

		private void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(_id);
			}
		}

		#endregion
	}
}
