// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	internal class AppState : TreeListNode<IAppState>, IAppState, IPresentableContext
	{
		#region data

		private static int _idCounter;

		private readonly AppStateService _stateManager;
		private readonly IPresentable _controller;
		private readonly string _id;
		private readonly string _deeplinkId;

		private IAsyncOperation _dismissOp;
		private bool _isActive;
		private bool _disposed;

		private AppViewController _tmpController;
		private PresentOptions _tmpControllerOptions;
		private PresentArgs _tmpControllerArgs;

		#endregion

		#region interface

		internal IAppViewService ViewManager => _stateManager.ViewManager;
		internal IPresentableFactory ControllerFactory => _stateManager.ControllerFactory;

		internal AppState(AppStateService stateManager, AppState parentState, Type controllerType, PresentOptions options, PresentArgs args)
			: base(parentState)
		{
			Debug.Assert(stateManager != null);
			Debug.Assert(controllerType != null);

			_tmpControllerOptions = options;
			_tmpControllerArgs = args;

			_id = Utility.GetNextId("_state", ref _idCounter);
			_deeplinkId = Utility.GetPresentableTypeId(controllerType);
			_stateManager = stateManager;
			_controller = stateManager.ControllerFactory.CreateController(controllerType, this);
			_stateManager.AddState(this);
		}

		internal void DismissInternal(ITraceable op)
		{
			op.TraceEvent(TraceEventType.Verbose, "DismissState " + _controller.TypeId);
			_controller.InvokeOnDismiss();
		}

		internal bool TryActivate(ITraceable op)
		{
			if (!_isActive && (Parent == null || Parent.IsActive))
			{
				op.TraceEvent(TraceEventType.Verbose, "ActivateState " + _controller.TypeId);

				_isActive = true;
				_controller.TryActivate();

				return true;
			}

			return false;
		}

		internal bool TryDeactivate(ITraceable op)
		{
			if (_isActive)
			{
				op.TraceEvent(TraceEventType.Verbose, "DeactivateState " + _controller.TypeId);

				try
				{
					_controller.TryDeactivate();
				}
				finally
				{
					_isActive = false;
				}

				return true;
			}

			return false;
		}

		internal AppView GetPrevView()
		{
			return Prev?.Controller.GetTopView();
		}

		internal AppViewController CreateController(AppViewController c, Type controllerType, PresentOptions options, PresentArgs args)
		{
			ThrowIfDisposed();

			_tmpControllerArgs = args;
			_tmpControllerOptions = options;
			_tmpController = c;

			try
			{
				var controller = _stateManager.ControllerFactory.CreateController(controllerType, this);
				c.AddChildController(controller);
				return controller;
			}
			finally
			{
				_tmpControllerArgs = null;
				_tmpControllerOptions = PresentOptions.None;
				_tmpController = null;
			}
		}

		/// <summary>
		/// Throws <see cref="ObjectDisposedException"/> if the instance is disposed.
		/// </summary>
		protected void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(_id);
			}
		}

		#endregion

		#region IAppState

		public IPresentable Controller => _controller;

		#endregion

		#region IPresentableContext

		PresentArgs IPresentableContext.PresentArgs => _tmpControllerArgs;

		IPresentable IPresentableContext.ParentController => _tmpController;

		IAppState IPresentableContext.ParentState => this;

		IAppView IPresentableContext.CreateView(IPresentable c)
		{
			ThrowIfDisposed();

			if (_tmpController == null)
			{
				return _stateManager.ViewManager.CreateView(c.Id, GetPrevView(), AppViewOptions.None);
			}
			else
			{
				return _stateManager.ViewManager.CreateView(c.Id, _tmpController.GetTopView(), AppViewOptions.None);
			}
		}

		IAsyncOperation IPresentableContext.DismissAsync(IPresentable controller)
		{
			ThrowIfDisposed();

			return _stateManager.DismissAsync(controller);
		}

		#endregion

		#region IPresenter

		public IAsyncOperation<IPresentable> PresentAsync(Type controllerType, PresentArgs args)
		{
			ThrowIfDisposed();
			return _stateManager.PresentAsync(this, controllerType, args);
		}

		public IAsyncOperation<TController> PresentAsync<TController>(PresentArgs args) where TController : IPresentable
		{
			ThrowIfDisposed();
			return _stateManager.PresentAsync<TController>(this, args);
		}

		#endregion

		#region IPresentable

		public string Id => _id;
		public IAppView View => _controller.View;
		public bool IsActive => _isActive;

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
		#endregion
	}
}
