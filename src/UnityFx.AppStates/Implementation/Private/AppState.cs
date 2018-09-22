// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityFx.AppStates.Common;
using UnityFx.Async;
using UnityFx.DependencyInjection;

namespace UnityFx.AppStates
{
	internal sealed class AppState : TreeListNode<IAppState>, IAppState, IPresentable
	{
		#region data

		private static int _idCounter;

		private readonly AppStateService _stateManager;
		private readonly PresentContext _controllerContext;
		private readonly IViewController _controller;
		private readonly string _id;
		private readonly string _deeplinkId;

		private IAsyncOperation _dismissOp;
		private bool _active;
		private bool _disposed;

		#endregion

		#region interface

		internal PresentContext PresentContext => _controllerContext;

		internal AppState(AppStateService stateManager, AppState parentState, Type controllerType, PresentArgs args)
			: base(parentState)
		{
			Debug.Assert(stateManager != null);
			Debug.Assert(controllerType != null);

			_id = Utility.GetNextId("_state", ref _idCounter);
			_deeplinkId = Utility.GetControllerTypeId(controllerType);
			_stateManager = stateManager;
			_stateManager.AddState(this);

			// Controller & view should be created after the state has been initialized.
			try
			{
				var serviceProvider = stateManager.ServiceProvider;
				var scope = serviceProvider.CreateScope();
				var controllerFactory = scope.ServiceProvider.GetService<IViewControllerFactory>();

				_controllerContext = new PresentContext(scope, this, null, args);

				if (controllerFactory != null)
				{
					_controller = controllerFactory.CreateController(controllerType, _controllerContext);
				}
				else
				{
					_controller = (IViewController)scope.ServiceProvider.CreateInstance(controllerType, _controllerContext);
				}
			}
			catch
			{
				_controllerContext?.Dispose();
				_stateManager.RemoveState(this);
				throw;
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
		public IViewController Controller => _controller;

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
			Debug.Assert(presentContext != null);
			Debug.Assert(!_disposed);

			if (_controller is IPresentable presentable)
			{
				return presentable.PresentAsync(presentContext);
			}

			return AsyncResult.CompletedOperation;
		}

		public IAsyncOperation DismissAsync(IDismissContext dismissContext)
		{
			Debug.Assert(dismissContext != null);
			Debug.Assert(!_disposed);

			if (_controller is IPresentable presentable)
			{
				return presentable.DismissAsync(dismissContext);
			}

			return AsyncResult.CompletedOperation;
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
					if (_controller is IDisposable d)
					{
						d.Dispose();
					}

					_controllerContext.Dispose();
				}
				finally
				{
					_stateManager.RemoveState(this);
				}
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
