// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Defines a wrapper data/services for a <see cref="IViewController"/>.
	/// </summary>
	/// <remarks>
	/// We want <see cref="IViewController"/> interface to be as minimalistic as possible. That's why we need to store
	/// controller context outside of actual controller. This class manages the controller created, provides its context
	/// (via <see cref="IPresentContext"/> interface) and serves as a proxy between the controller and
	/// <see cref="IPresentService"/> implementation.
	/// </remarks>
	internal class ViewControllerProxy : TreeListNode<ViewControllerProxy>, IPresentContext, IPresentResult, ICommandTarget
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

		private readonly PresentService _presenter;
		private readonly Type _controllerType;
		private readonly PresentArgs _presentArgs;
		private readonly PresentOptions _presentOptions;
		private readonly string _name;
		private readonly int _id;

		private IServiceProvider _serviceProvider;
		private IDisposable _scope;
		private IViewController _controller;
		private IView _view;

		private Task<IViewController> _presentTask;
		private State _state;

		#endregion

		#region interface

		internal PresentOptions PresentOptions => _presentOptions;

		internal ViewControllerProxy(PresentService presentManager, ViewControllerProxy parent, Type controllerType, PresentArgs args, int id)
			: base(parent)
		{
			Debug.Assert(presentManager != null);
			Debug.Assert(controllerType != null);
			Debug.Assert(args != null);

			_presenter = presentManager;
			_serviceProvider = presentManager.ServiceProvider;
			_controllerType = controllerType;
			_presentArgs = args;
			_presentOptions = args.Options;
			_name = Utility.GetControllerTypeId(controllerType);
			_id = id;
		}

		internal Task PresentAsync(object userState)
		{
			if (_serviceProvider.GetService(typeof(IViewFactory)) is IViewFactory viewFactory)
			{
				_presentTask = PresentInternal(viewFactory, userState);
				return _presentTask;
			}
			else
			{
				throw new NotSupportedException();
			}
		}

		internal bool TryActivate()
		{
			if (_state == State.Presented)
			{
				_state = State.Active;

				if (_controller is IViewControllerEvents controllerEvents)
				{
					controllerEvents.OnActivate();
				}

				return true;
			}

			return false;
		}

		internal bool TryDeactivate()
		{
			if (_state == State.Active)
			{
				_state = State.Presented;

				if (_controller is IViewControllerEvents controllerEvents)
				{
					controllerEvents.OnDeactivate();
				}

				return true;
			}

			return false;
		}

		internal bool TryDismiss()
		{
			if (_state == State.Presented)
			{
				_controller.Dispose();
				return true;
			}

			return false;
		}

		internal void DismissChildControllers()
		{
			var children = GetChildControllers();

			if (children != null)
			{
				foreach (var controller in children)
				{
					controller.Dismiss();
				}
			}
		}

		#endregion

		#region IViewControllerContext

		public int Id => _id;

		public bool IsActive => _state == State.Active;

		#endregion

		#region IPresenter

		public IPresentResult Present(Type controllerType)
		{
			Debug.Assert(!IsDismissed);
			return _presenter.Present(this, controllerType, PresentArgs.Default);
		}

		public IPresentResult Present(Type controllerType, PresentArgs args)
		{
			Debug.Assert(!IsDismissed);
			return _presenter.Present(this, controllerType, args);
		}

		public IPresentResult<TController> Present<TController>() where TController : class, IViewController
		{
			Debug.Assert(!IsDismissed);
			return _presenter.Present<TController>(this, PresentArgs.Default);
		}

		public IPresentResult<TController> Present<TController>(PresentArgs args) where TController : class, IViewController
		{
			Debug.Assert(!IsDismissed);
			return _presenter.Present<TController>(this, args);
		}

		#endregion

		#region IPresentResult

		public event EventHandler<PresentCompletedEventArgs> PresentCompleted;

		public event EventHandler Dismissed;

		public bool IsPresented => _state == State.Presented || _state == State.Active;

		public bool IsDismissed => _state == State.Dismissed || _state == State.Disposed;

		public Task<IViewController> PresentTask => _presentTask;

		public Task DismissTask => throw new NotImplementedException();

		public IViewController Controller => _controller;

		public void Dismiss()
		{
			if (_controller != null)
			{
				_controller.Dispose();
			}
			else
			{
				_state = State.Disposed;
			}
		}

		#endregion

		#region ICommandTarget

		public bool InvokeCommand(string commandName, object args)
		{
			if ((_state == State.Presented || _state == State.Active) && _controller is ICommandTarget cmdTarget)
			{
				return cmdTarget.InvokeCommand(commandName, args);
			}

			return false;
		}

		#endregion

		#region IServiceProvider

		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(IPresentContext) || serviceType == typeof(IPresenter) || serviceType == typeof(IServiceProvider))
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
				_state = State.Disposed;

				try
				{
					_controller?.Dispose();
					_view?.Dispose();
				}
				finally
				{
					_scope?.Dispose();
				}
			}
		}

		#endregion

		#region implementation

		private async Task<IViewController> PresentInternal(IViewFactory viewFactory, object userState)
		{
			try
			{
				_view = await viewFactory.CreateViewAsync(_controllerType, GetIndex());

				if (_state != State.Initialized)
				{
					_view.Dispose();
					throw new OperationCanceledException();
				}

				if (_serviceProvider.GetService(typeof(IViewControllerFactory)) is IViewControllerFactory controllerFactory)
				{
					_scope = controllerFactory.CreateControllerScope(ref _serviceProvider);
					_controller = controllerFactory.CreateController(_controllerType, this, _presentArgs, _view);
				}
				else
				{
					_controller = (IViewController)ActivatorUtilities.CreateInstance(_serviceProvider, _controllerType, this, _presentArgs, _view);
				}

				Debug.Assert(_controller != null);

				_controller.Disposed += OnDismissed;
				_state = State.Presented;
			}
			catch (Exception e)
			{
				_controller?.Dispose();
				_scope?.Dispose();
				_view?.Dispose();

				PresentCompleted?.Invoke(this, new PresentCompletedEventArgs(e, userState));
				throw;
			}

			PresentCompleted?.Invoke(this, new PresentCompletedEventArgs(_controller, userState));
			return _controller;
		}

		private void OnDismissed(object sender, EventArgs e)
		{
			TryDeactivate();

			if (_state != State.Dismissed && _state != State.Disposed)
			{
				_state = State.Dismissed;
				_scope?.Dispose();
				_presenter.Dismissed(this);
			}
		}

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
