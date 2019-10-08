// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Defines a wrapper data/services for a <see cref="IViewController"/>.
	/// </summary>
	/// <remarks>
	/// We want <see cref="IViewController"/> interface to be as minimalistic as possible. That's why we need to store
	/// controller context outside of actual controller. This class manages the controller created, provides its context
	/// (via <see cref="IPresentContext"/> interface) and serves as a proxy between the controller and user.
	/// </remarks>
	internal class PresentResult<TController, TResult> : TaskCompletionSource<TResult>, IPresentContext<TResult>, IPresentResult<TController, TResult>, IPresentable where TController : class, IViewController
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

		private readonly Presenter _presenter;
		private readonly Type _controllerType;
		private readonly PresentArgs _presentArgs;
		private readonly PresentOptions _presentOptions;
		private readonly IPresentable _parent;
		private readonly int _id;

		private IServiceProvider _serviceProvider;
		private IDisposable _scope;
		private TController _controller;
		private IView _view;

		private State _state;

		#endregion

		#region interface

		internal PresentOptions PresentOptions => _presentOptions;

		internal PresentResult(Presenter presenter, IPresentable parent, Type controllerType, PresentArgs args, int id)
			: base(parent)
		{
			Debug.Assert(presenter != null);
			Debug.Assert(controllerType != null);
			Debug.Assert(args != null);

			_presenter = presenter;
			_parent = parent;
			_serviceProvider = presenter.ServiceProvider;
			_controllerType = controllerType;
			_presentArgs = args;
			_presentOptions = args.Options;
			_id = id;
		}

		internal async Task<TController> PresentAsync(IViewFactory viewFactory, int index)
		{
			try
			{
				_view = await viewFactory.CreateViewAsync(_controllerType, index);

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
					_controller = (TController)controllerFactory.CreateController(_controllerType, this, _presentArgs, _view);
				}
				else
				{
					_controller = (TController)ActivatorUtilities.CreateInstance(_serviceProvider, _controllerType, this, _presentArgs, _view);
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

		#endregion

		#region IPresentable

		public IPresentable Parent => _parent;

		public bool TryActivate()
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

		public bool TryDeactivate()
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

		public void DismissChild()
		{
			if (_state != State.Dismissed && _state != State.Disposed)
			{
				try
				{
					if (_controller is IViewControllerEvents c)
					{
						if (_state == State.Active)
						{
							c.OnDeactivate();
							c.OnDismiss();
						}
						else if (_state == State.Presented)
						{
							c.OnDismiss();
						}
					}
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
				finally
				{
					_state = State.Dismissed;
				}
			}
		}

		public void DisposeChild()
		{
			if (_state != State.Disposed)
			{
				DismissInternal(default, true);
			}
		}

		#endregion

		#region IPresentContext

		public PresentArgs Args => _presentArgs;

		public IView View => _view;

		public bool IsActive => _state == State.Active;

		public bool IsDismissed => _state == State.Dismissed || _state == State.Disposed;

		public void Dismiss(TResult result)
		{
			Dismiss(result, false);
		}

		public void Dismiss()
		{
			Dismiss(default, false);
		}

		#endregion

		#region IPresenter

		public IPresentResult PresentAsync(Type controllerType, PresentArgs args)
		{
			ThrowIfDisposed();
			return _presenter.PresentAsync(this, controllerType, args);
		}

		public IPresentResult<TController2> PresentAsync<TController2>(PresentArgs args) where TController2 : IViewController
		{
			ThrowIfDisposed();
			return _presenter.PresentAsync<TController2>(this, args);
		}

		public IPresentResult<TController2, TResult2> PresentAsync<TController2, TResult2>(PresentArgs args) where TController2 : IViewController
		{
			ThrowIfDisposed();
			return _presenter.PresentAsync<TController2, TResult2>(this, args);
		}

		#endregion

		#region IPresentResult

		IViewController IPresentResult.Controller => _controller;

		Task IPresentResult.Task => Task;

		public TController Controller => _controller;

		public TResult Result => Task.Result;

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
			Dismiss(default, false);
		}

		#endregion

		#region implementation

		private void Dismiss(TResult result, bool cancelled)
		{
			if (_state != State.Disposed)
			{
				if (_state == State.Dismissed)
				{
					DismissInternal(result, cancelled);
				}
				else
				{
					try
					{
						_presenter.DismissChildren(this);
					}
					finally
					{
						DismissInternal(result, cancelled);
					}
				}
			}
		}

		private void DismissInternal(TResult result, bool cancelled)
		{
			try
			{
				_state = State.Disposed;

				if (cancelled)
				{
					TrySetCanceled();
				}
				else
				{
					TrySetResult(result);
				}

				if (_controller is IDisposable d)
				{
					d.Dispose();
				}

				_view?.Dispose();
				_scope?.Dispose();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		private void OnDismissed(object sender, EventArgs e)
		{
			Dismiss(default, true);
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
