// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

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
	internal class PresentableProxy : TreeListNode<PresentableProxy>, IPresentContext, IPresentResult, IPresentableEvents, ICommandTarget
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
		private readonly IServiceProvider _serviceProvider;
		private readonly IDisposable _scope;
		private readonly IPresentable _controller;
		private readonly PresentOptions _presentOptions;
		private readonly string _name;
		private readonly int _id;

		private State _state;

		#endregion

		#region interface

		internal PresentableProxy(PresentService presentManager, PresentableProxy parent, Type controllerType, PresentArgs args, int id)
			: base(parent)
		{
			Debug.Assert(presentManager != null);
			Debug.Assert(controllerType != null);
			Debug.Assert(args != null);

			_presenter = presentManager;
			_serviceProvider = presentManager.ServiceProvider;
			_presentOptions = args.Options;
			_name = Utility.GetControllerTypeId(controllerType);
			_id = id;

			// Controller should be created after the proxy has been initialized.
			try
			{
				if (_serviceProvider.GetService(typeof(IViewControllerFactory)) is IViewControllerFactory controllerFactory)
				{
					_scope = controllerFactory.CreateControllerScope(ref _serviceProvider);
					_controller = (IPresentable)controllerFactory.CreateController(controllerType, this, args);
				}
				else
				{
					_controller = (IPresentable)ActivatorUtilities.CreateInstance(_serviceProvider, controllerType, this, args);
				}
			}
			catch
			{
				_scope?.Dispose();
				throw;
			}
		}

		internal bool TryActivate()
		{
			if (_state == State.Presented)
			{
				OnActivate();
				return true;
			}

			return false;
		}

		internal bool TryDeactivate()
		{
			if (_state == State.Active)
			{
				OnDeactivate();
				return true;
			}

			return false;
		}

		internal bool TryDismiss()
		{
			if (_state == State.Presented)
			{
				OnDismiss();
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

		public string ControllerName => _name;

		public string ViewName => throw new NotImplementedException();

		public bool IsActive => _state == State.Active;

		public bool IsModal => (_presentOptions & PresentOptions.Modal) != 0;

		#endregion

		#region IPresenter

		public IPresentResult Present(Type controllerType)
		{
			Debug.Assert(_state != State.Disposed);
			return _presenter.Present(this, controllerType, PresentArgs.Default);
		}

		public IPresentResult Present(Type controllerType, PresentArgs args)
		{
			Debug.Assert(_state != State.Disposed);
			return _presenter.Present(this, controllerType, args);
		}

		public IPresentResult<TController> Present<TController>() where TController : class, IPresentable
		{
			Debug.Assert(_state != State.Disposed);
			return _presenter.Present<TController>(this, PresentArgs.Default);
		}

		public IPresentResult<TController> Present<TController>(PresentArgs args) where TController : class, IPresentable
		{
			Debug.Assert(_state != State.Disposed);
			return _presenter.Present<TController>(this, args);
		}

		#endregion

		#region IPresentResult

		public IPresentable Controller => _controller;

		#endregion

		#region IDismissable

		public event EventHandler Dismissed;

		public bool IsDismissed => _state == State.Dismissed || _state == State.Disposed;

		public void Dismiss()
		{
			if (_state != State.Dismissed && _state != State.Disposed)
			{
				_presenter.Dismiss(this);
			}
		}

		#endregion

		#region IPresentableEvents

		public void OnPresent()
		{
			Debug.Assert(_state == State.Initialized);

			_state = State.Presented;

			if (_controller is IPresentableEvents controllerEvents)
			{
				controllerEvents.OnPresent();
			}
		}

		public void OnActivate()
		{
			Debug.Assert(_state == State.Presented);

			_state = State.Active;

			if (_controller is IPresentableEvents controllerEvents)
			{
				controllerEvents.OnActivate();
			}
		}

		public void OnDeactivate()
		{
			Debug.Assert(_state == State.Active);

			_state = State.Presented;

			if (_controller is IPresentableEvents controllerEvents)
			{
				controllerEvents.OnDeactivate();
			}
		}

		public void OnDismiss()
		{
			Debug.Assert(_state == State.Presented);

			_state = State.Dismissed;

			if (_controller is IPresentableEvents controllerEvents)
			{
				controllerEvents.OnDismiss();
			}

			Dismissed?.Invoke(this, EventArgs.Empty);
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

		#region IServiceProvider

		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(IPresentContext))
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
					_controller.Dispose();
				}
				finally
				{
					_scope?.Dispose();
				}
			}
		}

		#endregion

		#region implementation

		private Stack<PresentableProxy> GetChildControllers()
		{
			var result = default(Stack<PresentableProxy>);
			var nextState = Next;

			while (nextState != null)
			{
				if (nextState.Parent == this)
				{
					if (result == null)
					{
						result = new Stack<PresentableProxy>();
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
