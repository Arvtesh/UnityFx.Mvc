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
	internal class PresentableProxy : TreeListNode<PresentableProxy>, IPresentContext, IPresentResult, IPresentableEvents, ICommandTarget, IDisposable
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

			_mvcService = presentManager;
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

		public int Id => _id;

		public string ControllerName => _name;

		public string ViewName => throw new NotImplementedException();

		public bool IsActive => _state == State.Active;

		public bool IsModal => (_presentOptions & PresentOptions.Modal) != 0;

		public void Dismiss()
		{
			Dispose();
		}

		#endregion

		#region IPresenter

		public IPresentResult Present(Type controllerType)
		{
			Debug.Assert(_state != State.Disposed);
			return _mvcService.Present(this, controllerType, PresentArgs.Default);
		}

		public IPresentResult Present(Type controllerType, PresentArgs args)
		{
			Debug.Assert(_state != State.Disposed);
			return _mvcService.Present(this, controllerType, args);
		}

		public IPresentResult<TController> Present<TController>() where TController : class, IViewController
		{
			Debug.Assert(_state != State.Disposed);
			return _mvcService.Present<TController>(this, PresentArgs.Default);
		}

		public IPresentResult<TController> Present<TController>(PresentArgs args) where TController : class, IViewController
		{
			Debug.Assert(_state != State.Disposed);
			return _mvcService.Present<TController>(this, args);
		}

		#endregion

		#region IPresentResult

		public IPresentable Controller => _controller;

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
				if (_state == State.Presented)
				{
					OnDismiss();
				}

				_state = State.Disposed;

				try
				{
					_controller.Dispose();
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
