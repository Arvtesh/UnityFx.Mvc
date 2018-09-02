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
	internal sealed class AppState : TreeListNode<IAppState>, IAppState, IPresentableContext
	{
		#region data

		private static int _idCounter;

		private readonly AppStateService _stateManager;
		private readonly IPresentable _controller;
		private readonly string _id;
		private readonly string _deeplinkId;

		private IAsyncOperation _dismissOp;
		private bool _disposed;

		private PresentArgs _tmpControllerArgs;

		#endregion

		#region interface

		internal IAppViewService ViewManager => _stateManager.ViewManager;
		internal IPresentableFactory ControllerFactory => _stateManager.ControllerFactory;

		internal AppState(AppStateService stateManager, AppState parentState, Type controllerType, PresentArgs args)
			: base(parentState)
		{
			Debug.Assert(stateManager != null);
			Debug.Assert(controllerType != null);

			_tmpControllerArgs = args;

			_id = Utility.GetNextId("_state", ref _idCounter);
			_deeplinkId = Utility.GetPresentableTypeId(controllerType);
			_stateManager = stateManager;
			_controller = stateManager.ControllerFactory.CreateController(controllerType, this);
			_stateManager.AddState(this);
		}

		#endregion

		#region IAppState

		public IPresentable Controller => _controller;

		#endregion

		#region IPresentableContext

		PresentArgs IPresentableContext.PresentArgs => _tmpControllerArgs;

		IAppState IPresentableContext.ParentState => this;

		IPresentable IPresentableContext.ParentController => null;

		IAppViewService IPresentableContext.ViewManager => _stateManager.ViewManager;

		#endregion

		#region IPresenter

		public IAsyncOperation<IPresentable> PresentAsync(Type controllerType)
		{
			ThrowIfDisposed();
			return _stateManager.PresentAsync(this, controllerType, PresentArgs.Default);
		}

		public IAsyncOperation<IPresentable> PresentAsync(Type controllerType, PresentArgs args)
		{
			ThrowIfDisposed();
			return _stateManager.PresentAsync(this, controllerType, args);
		}

		public IAsyncOperation<TController> PresentAsync<TController>() where TController : class, IPresentable
		{
			ThrowIfDisposed();
			return _stateManager.PresentAsync<TController>(this, PresentArgs.Default);
		}

		public IAsyncOperation<TController> PresentAsync<TController>(PresentArgs args) where TController : class, IPresentable
		{
			ThrowIfDisposed();
			return _stateManager.PresentAsync<TController>(this, args);
		}

		#endregion

		#region IPresentable

		public string Id => _id;
		public IAppView View => _controller.View;
		public bool IsActive => _controller.IsActive;

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

		/// <inheritdoc/>
		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				_stateManager.RemoveState(this);
				_controller.Dispose();
			}
		}

		#endregion

		#region implementation

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
