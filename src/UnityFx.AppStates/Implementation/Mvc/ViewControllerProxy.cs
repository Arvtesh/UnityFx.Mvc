// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Defines a wrapper data/services for a <see cref="IViewController"/>.
	/// </summary>
	/// <remarks>
	/// We want <see cref="IViewController"/> interface to be as minimalistic as possible. That's why we need to store
	/// controller context outside of actual controller. This class manages the controller created, provides its context
	/// for it (via implementation of <see cref="IViewControllerContext"/> and injecting it into the controller) and serves
	/// as a proxy between a parent state/controller and the owned one.
	/// </remarks>
	internal class ViewControllerProxy : IViewControllerContext, IPresentContext, IDismissContext, IPresentable, IPresentableEvents, IDisposable
	{
		#region data

		private readonly IAppStateService _stateManager;
		private readonly IViewFactory _viewFactory;
		private readonly IServiceProvider _serviceProvider;
		private readonly IDisposable _scope;
		private readonly IAppState _parentState;
		private readonly IViewController _parentController;
		private readonly IViewController _controller;
		private readonly PresentArgs _args;

		private bool _disposed;

		#endregion

		#region interface

		public IViewController Controller => _controller;

		public ViewControllerProxy(AppStateService stateManager, IAppState parentState, IViewController parentController, Type controllerType, PresentArgs args)
		{
			Debug.Assert(stateManager != null);
			Debug.Assert(parentState != null);
			Debug.Assert(controllerType != null);
			Debug.Assert(args != null);

			_stateManager = stateManager;
			_serviceProvider = stateManager.ServiceProvider;
			_viewFactory = stateManager.ViewFactory;
			_parentState = parentState;
			_parentController = parentController;
			_args = args;

			// Controller should be created after the proxy has been initialized.
			try
			{
				if (_serviceProvider.GetService(typeof(IViewControllerFactory)) is IViewControllerFactory controllerFactory)
				{
					_scope = controllerFactory.CreateControllerScope(ref _serviceProvider);
					_controller = controllerFactory.CreateController(controllerType, this);
				}
				else
				{
					_controller = (IViewController)Utility.CreateInstance(_serviceProvider, controllerType, this);
				}
			}
			catch
			{
				_scope?.Dispose();
				throw;
			}
		}

		#endregion

		#region IViewControllerContext

		public PresentArgs PresentArgs => _args;

		public IServiceProvider ServiceProvider => _serviceProvider;

		public IAppState ParentState => _parentState;

		public IViewController ParentController => _parentController;

		public IAsyncOperation<IView> LoadViewAsync()
		{
			Debug.Assert(!_disposed);

			var viewId = Utility.GetViewResourceId(_controller.GetType());
			var insertAfter = _parentState.Prev?.Controller.View;

			return _viewFactory.LoadViewAsync(viewId, insertAfter);
		}

		public IAsyncOperation<IViewController> PresentAsync(Type controllerType, PresentArgs args)
		{
			Debug.Assert(!_disposed);
			return _parentState.PresentAsync(controllerType, args);
		}

		public IAsyncOperation<TController> PresentAsync<TController>(PresentArgs args) where TController : class, IViewController
		{
			Debug.Assert(!_disposed);
			return _parentState.PresentAsync<TController>(args);
		}

		public IAsyncOperation DismissAsync()
		{
			Debug.Assert(!_disposed);
			return _parentState.DismissAsync();
		}

		#endregion

		#region IPresentContext

		public IAppState PrevState => null;

		#endregion

		#region IDismissContext

		public IAppState NextState => null;

		#endregion

		#region IPresentable

		public IAsyncOperation PresentAsync(IPresentContext presentContext)
		{
			Debug.Assert(presentContext == null);
			Debug.Assert(!_disposed);

			if (_controller is IPresentable presentable)
			{
				// Make sure the method never returns null.
				var op = presentable.PresentAsync(this);
				return op ?? AsyncResult.CompletedOperation;
			}

			return AsyncResult.CompletedOperation;
		}

		public IAsyncOperation DismissAsync(IDismissContext dismissContext)
		{
			Debug.Assert(dismissContext == null);
			Debug.Assert(!_disposed);

			if (_controller is IPresentable presentable)
			{
				// Make sure the method never returns null.
				var op = presentable.DismissAsync(this);
				return op ?? AsyncResult.CompletedOperation;
			}

			return AsyncResult.CompletedOperation;
		}

		#endregion

		#region IPresentableEvents

		public void OnPresent()
		{
			Debug.Assert(!_disposed);

			if (_controller is IPresentableEvents controllerEvents)
			{
				controllerEvents.OnPresent();
			}
		}

		public void OnActivate()
		{
			Debug.Assert(!_disposed);

			if (_controller is IPresentableEvents controllerEvents)
			{
				controllerEvents.OnActivate();
			}
		}

		public void OnDeactivate()
		{
			Debug.Assert(!_disposed);

			if (_controller is IPresentableEvents controllerEvents)
			{
				controllerEvents.OnDeactivate();
			}
		}

		public void OnDismiss()
		{
			Debug.Assert(!_disposed);

			if (_controller is IPresentableEvents controllerEvents)
			{
				controllerEvents.OnDismiss();
			}
		}

		#endregion

		#region ISynchronizeInvoke

		public bool InvokeRequired => _stateManager.InvokeRequired;

		public IAsyncResult BeginInvoke(Delegate method, object[] args) => _stateManager.BeginInvoke(method, args);

		public object EndInvoke(IAsyncResult result) => _stateManager.EndInvoke(result);

		public object Invoke(Delegate method, object[] args) => _stateManager.Invoke(method, args);

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
				}
				finally
				{
					_scope?.Dispose();
				}
			}
		}

		#endregion
	}
}
