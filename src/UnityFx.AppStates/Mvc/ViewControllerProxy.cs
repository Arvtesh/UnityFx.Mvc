// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityFx.Async;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Defines a wrapper data/services for a <see cref="IViewController"/>.
	/// </summary>
	/// <remarks>
	/// We want <see cref="IViewController"/> interface to be as minimalistic as possible. That's why we need to store
	/// controller context outside of actual controller. This class manages the controller created, provides its context
	/// (via <see cref="IViewControllerContext"/> interface) and serves as a proxy between the controller and
	/// <see cref="IPresentService"/> implementation.
	/// </remarks>
	internal class ViewControllerProxy : TreeListNode<ViewControllerProxy>, IViewControllerContext, IPresentContext, IDismissContext, IAsyncPresentable, IPresentableEvents, ICommandTarget, IDisposable
	{
		#region data

		private enum State
		{
			Initialized,
			Presented,
			Active,
			Dismissed,
			Disposed
		}

		private readonly PresentService _mvcService;
		private readonly IServiceProvider _serviceProvider;
		private readonly IDisposable _scope;
		private readonly IViewController _controller;
		private readonly PresentOptions _presentOptions;
		private readonly string _name;

		private State _state;

		#endregion

		#region interface

		public IViewController Controller => _controller;

		public ViewControllerProxy(PresentService stateManager, ViewControllerProxy parent, Type controllerType, PresentArgs args)
			: base(parent)
		{
			Debug.Assert(stateManager != null);
			Debug.Assert(controllerType != null);
			Debug.Assert(args != null);

			_mvcService = stateManager;
			_serviceProvider = stateManager.ServiceProvider;
			_presentOptions = args.Options;
			_name = Utility.GetControllerTypeId(controllerType);

			// Controller should be created after the proxy has been initialized.
			try
			{
				if (_serviceProvider.GetService(typeof(IViewControllerFactory)) is IViewControllerFactory controllerFactory)
				{
					_scope = controllerFactory.CreateControllerScope(ref _serviceProvider);
					_controller = controllerFactory.CreateController(controllerType, this, args);
				}
				else
				{
					_controller = (IViewController)ActivatorUtilities.CreateInstance(_serviceProvider, controllerType, this, args);
				}
			}
			catch
			{
				_scope?.Dispose();
				throw;
			}
		}

		internal void DismissChildControllers()
		{
			var children = GetChildControllers();

			if (children != null)
			{
				foreach (var controller in children)
				{
					controller.Dispose();
				}
			}
		}

		#endregion

		#region IViewControllerContext

		public string Name => _name;

		public bool IsActive => _state == State.Active;

		public bool IsModal => (_presentOptions & PresentOptions.Modal) != 0;

		public IAsyncOperation<IViewController> PresentAsync(Type controllerType, PresentArgs args)
		{
			Debug.Assert(_state == State.Presented || _state == State.Active);
			return _mvcService.PresentAsync(this, controllerType, args);
		}

		public IAsyncOperation<TController> PresentAsync<TController>(PresentArgs args) where TController : class, IViewController
		{
			Debug.Assert(_state == State.Presented || _state == State.Active);
			return _mvcService.PresentAsync<TController>(this, args);
		}

		public IAsyncOperation DismissAsync()
		{
			Debug.Assert(_state == State.Presented || _state == State.Active);
			return _mvcService.DismissAsync(this);
		}

		#endregion

		#region IPresentContext

		public IViewController PrevController => null;

		#endregion

		#region IDismissContext

		public IViewController NextController => null;

		#endregion

		#region IPresentable

		public IAsyncOperation PresentAsync(IPresentContext presentContext)
		{
			Debug.Assert(presentContext == null);
			Debug.Assert(_state == State.Initialized);

			if (_controller is IAsyncPresentable presentable)
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
			Debug.Assert(_state == State.Presented);

			if (_controller is IAsyncPresentable presentable)
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
			Debug.Assert(_state == State.Initialized);

			_mvcService.TraceEvent(TraceEventType.Verbose, "Present " + _name);
			_state = State.Presented;

			if (_controller is IPresentableEvents controllerEvents)
			{
				controllerEvents.OnPresent();
			}
		}

		public void OnActivate()
		{
			Debug.Assert(_state == State.Presented);

			_mvcService.TraceEvent(TraceEventType.Verbose, "Activate " + _name);
			_state = State.Active;

			if (_controller is IPresentableEvents controllerEvents)
			{
				controllerEvents.OnActivate();
			}
		}

		public void OnDeactivate()
		{
			Debug.Assert(_state == State.Active);

			try
			{
				if (_controller is IPresentableEvents controllerEvents)
				{
					controllerEvents.OnDeactivate();
				}
			}
			finally
			{
				_mvcService.TraceEvent(TraceEventType.Verbose, "Deactivate " + _name);
				_state = State.Presented;
			}
		}

		public void OnDismiss()
		{
			Debug.Assert(_state == State.Presented);

			try
			{
				if (_controller is IPresentableEvents controllerEvents)
				{
					controllerEvents.OnDismiss();
				}
			}
			finally
			{
				_mvcService.TraceEvent(TraceEventType.Verbose, "Dismiss " + _name);
				_state = State.Dismissed;
			}
		}

		#endregion

		#region ICommandTarget

		public bool InvokeCommand(string commandName, object args)
		{
			Debug.Assert(_state == State.Presented || _state == State.Active);

			if (_controller is ICommandTarget cmdTarget)
			{
				return cmdTarget.InvokeCommand(commandName, args);
			}

			return false;
		}

		#endregion

		#region ISynchronizeInvoke

		public bool InvokeRequired => _mvcService.InvokeRequired;

		public IAsyncResult BeginInvoke(Delegate method, object[] args) => _mvcService.BeginInvoke(method, args);

		public object EndInvoke(IAsyncResult result) => _mvcService.EndInvoke(result);

		public object Invoke(Delegate method, object[] args) => _mvcService.Invoke(method, args);

		#endregion

		#region IServiceProvider

		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(IViewControllerContext))
			{
				return this;
			}

			return _serviceProvider.GetService(serviceType);
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (_state != State.Disposed)
			{
				if (_state == State.Presented)
				{
					OnDismiss();
				}

				_state = State.Disposed;

				try
				{
					if (_controller is IDisposable d)
					{
						d.Dispose();
					}
				}
				finally
				{
					_mvcService.RemoveController(this);
					_scope?.Dispose();
				}
			}
		}

		#endregion

		#region implementation

		private Stack<ViewControllerProxy> GetChildControllers()
		{
			var result = default(Stack<ViewControllerProxy>);
			var nextState = Next;

			while (nextState != null)
			{
				if (nextState.Parent == this)
				{
					if (result == null)
					{
						result = new Stack<ViewControllerProxy>();
					}

					result.Push(nextState);
				}

				nextState = nextState.Next;
			}

			return result;
		}

		#endregion
	}
}
