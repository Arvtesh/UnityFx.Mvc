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
	internal class PresentResult : TreeListNode<PresentResult>, IPresentContext, IPresentResult, ICommandTarget
	{
		#region data

		private enum State
		{
			Initialized,
			Presented,
			Active,
			Disposed
		}

		private readonly Presenter _presenter;
		private readonly Type _controllerType;
		private readonly PresentArgs _presentArgs;
		private readonly PresentOptions _presentOptions;
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

		internal PresentResult(Presenter presenter, PresentResult parent, Type controllerType, PresentArgs args, int id)
			: base(parent)
		{
			Debug.Assert(presenter != null);
			Debug.Assert(controllerType != null);
			Debug.Assert(args != null);

			_presenter = presenter;
			_serviceProvider = presenter.ServiceProvider;
			_controllerType = controllerType;
			_presentArgs = args;
			_presentOptions = args.Options;
			_id = id;
		}

		internal Task PresentAsync(object userState)
		{
			if (_serviceProvider.GetService(typeof(IViewFactory)) is IViewFactory viewFactory)
			{
				_presentTask = PresentAsync(viewFactory, userState);
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

				if (_controller is IViewControllerEvents c)
				{
					c.OnActivate();
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

				if (_controller is IViewControllerEvents c)
				{
					c.OnDeactivate();
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

		#region IPresentContext

		public PresentArgs Args => _presentArgs;

		public IView View => _view;

		public bool IsActive => _state == State.Active;

		public bool IsDismissed => _state == State.Disposed;

		public void Dismiss()
		{
			Dispose();
		}

		#endregion

		#region IPresenter

		public IPresentResult PresentAsync(Type controllerType, PresentArgs args)
		{
			ThrowIfDisposed();
			return _presenter.PresentAsync(this, controllerType, args);
		}

		public IPresentResult<TController> PresentAsync<TController>(PresentArgs args) where TController : IViewController
		{
			ThrowIfDisposed();
			return _presenter.PresentAsync<TController>(this, args);
		}

		public IPresentResult<TController, TResult> PresentAsync<TController, TResult>(PresentArgs args) where TController : IViewController<TResult>
		{
			ThrowIfDisposed();
			return _presenter.PresentAsync<TController, TResult>(this, args);
		}

		#endregion

		#region IPresentResult

		public IViewController Controller => _controller;

		public Task Task => null;

		#endregion

		#region ICommandTarget

		public bool InvokeCommand(string commandName, object args)
		{
			ThrowIfDisposed();

			if (commandName is null)
			{
				throw new ArgumentNullException(nameof(commandName));
			}

			if (_state != State.Presented && _state != State.Active)
			{
				throw new InvalidOperationException();
			}

			return _controller.InvokeCommand(commandName, args);
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
					if (_controller is IViewControllerEvents c)
					{
						c.OnDismiss();
					}
				}
				finally
				{
					_view?.Dispose();
					_scope?.Dispose();
				}
			}
		}

		#endregion

		#region implementation

		private async Task<IViewController> PresentAsync(IViewFactory viewFactory, object userState)
		{
			try
			{
				_view = await viewFactory.CreateViewAsync(_controllerType, GetIndex());

				if (_view is null)
				{
					throw new InvalidOperationException();
				}

				if (_state != State.Initialized)
				{
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

				if (_controller is null)
				{
					throw new InvalidOperationException();
				}

				_view.Disposed += OnDismissed;
				_state = State.Presented;
			}
			catch
			{
				_scope?.Dispose();
				_view?.Dispose();
				throw;
			}

			return _controller;
		}

		private void OnDismissed(object sender, EventArgs e)
		{
			TryDeactivate();
			Dispose();
		}

		private Stack<PresentResult> GetChildControllers()
		{
			var result = default(Stack<PresentResult>);
			var nextState = Next;

			while (nextState != null)
			{
				if (nextState.Parent == this)
				{
					if (result == null)
					{
						result = new Stack<PresentResult>();
					}

					result.Push(nextState);
				}

				nextState = nextState.Next;
			}

			return result;
		}

		private void ThrowIfDisposed()
		{
			if (_state == State.Disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		#endregion
	}
}
