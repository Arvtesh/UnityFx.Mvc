// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Defines a wrapper data/services for a <see cref="IViewController"/>.
	/// </summary>
	internal abstract class PresentResult : IPresentResult, IPresenter, IViewControllerInfo, IEnumerator
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
		private readonly PresentResult _parent;
		private readonly Type _controllerType;
		private readonly Type _viewType;
		private readonly Type _resultType;
		private readonly Type _argsType;
		private readonly int _id;
		private readonly int _tag;
		private readonly object _viewKey;
		private readonly string _deeplinkId;

		private IServiceProvider _serviceProvider;
		private IDisposable _scope;
		private IViewController _controller;
		private IPresentEvents _presentEvents;
		private IView _view;

		private float _timer;
		private State _state;

		private CancellationToken _cancellationToken;
		private Exception _exception;

		#endregion

		#region interface

		internal PresentResult Parent => _parent;

		internal Exception Exception => _exception;

		internal bool IsFaulted => _exception != null;

		internal bool IsCancelled => _cancellationToken.IsCancellationRequested;

		protected CancellationToken CancellationToken => _cancellationToken;

		protected bool IsCompleted => _state == State.Disposed;

		protected PresentResult(Presenter presenter, PresentResultArgs context)
		{
			Debug.Assert(presenter != null);
			Debug.Assert(context != null);

			_presenter = presenter;
			_id = context.Id;
			_tag = context.Tag;
			_parent = context.Parent;
			_serviceProvider = presenter.ServiceProvider;
			_controllerType = context.ControllerType;
			_resultType = context.ResultType;
			_argsType = context.ArgsType;
			_viewType = context.ViewType;
			_deeplinkId = PresentUtilities.GetControllerDeeplinkId(_controllerType);
			_viewKey = presenter.ControllerBindings.GetViewKey(_controllerType);
		}

		internal bool TryActivate()
		{
			if (_state == State.Presented)
			{
				try
				{
					_presentEvents?.OnActivate();
					_state = State.Active;
					return true;
				}
				catch (Exception e)
				{
					_presenter.ReportError(e);
				}
			}

			return false;
		}

		internal void Deactivate()
		{
			if (_state == State.Active)
			{
				try
				{
					_state = State.Presented;
					_presentEvents?.OnDeactivate();
				}
				catch (Exception e)
				{
					_presenter.ReportError(e);
				}
			}
		}

		internal async Task PresentAsyc(PresentArgs presentArgs)
		{
			// 1) Create view.
			_view = await _presenter.ViewFactory.CreateViewAsync(_viewKey, presentArgs.Transform);

			// 1.1) Validate the reference returned.
			if (_view is null)
			{
				throw new PresentException(this, Messages.Format_ViewIsNull());
			}

			if (!_viewType.IsAssignableFrom(_view.GetType()))
			{
				throw new PresentException(this, Messages.Format_InvalidViewType(_viewType));
			}

			// 1.2) Make sure the controller we're not dismissed.
			if (IsDismissed)
			{
				throw new OperationCanceledException();
			}

			// 2) Configure the created view.
			ActivatorUtilities.TryConfigure(_view, presentArgs);

			// 3) Create the controller.
			_scope = _presenter.ControllerFactory.CreateScope(ref _serviceProvider);
			_controller = _presenter.ControllerFactory.CreateViewController(_controllerType, this, _view, presentArgs, presentArgs.UserData);
			_presentEvents = _controller as IPresentEvents;

			// 4) Fade-in the view.
			if (_view is IFadeable asyncPresentable)
			{
				await asyncPresentable.FadeInAsync(presentArgs.PresentOptions);
			}

			// 5) Finish present flow.
			_presentEvents?.OnPresent();

			if (_view is INotifyDisposed nd)
			{
				nd.Disposed += OnDismissed;
			}

			if (_view is INotifyCommand nc)
			{
				nc.Command += OnCommand;
			}

			_timer = Time.realtimeSinceStartup;
			_state = State.Presented;
		}

		protected async Task DismissAsync(bool animated)
		{
			Debug.Assert(_state < State.Dismissed);

			// Deactivate
			if (_state == State.Active)
			{
				try
				{
					_presentEvents?.OnDeactivate();
				}
				catch (Exception e)
				{
					_presenter.ReportError(e);
				}

				_state = State.Presented;
			}

			// Dismiss
			if (_state == State.Presented)
			{
				// Dismiss children
				try
				{
					foreach (var child in _presenter.GetChildren(this))
					{
						if (!child.IsDismissed)
						{
							child._exception = _exception;
							child._cancellationToken = _cancellationToken;
							await child.DismissAsync(false);
						}
					}
				}
				catch (Exception e)
				{
					_presenter.ReportError(e);
				}

				// Dismiss self
				_state = State.Dismissed;
				_timer = Time.realtimeSinceStartup - _timer;

				if (_controller is IPresentEvents presentEvents)
				{
					try
					{
						presentEvents.OnDismiss();
					}
					catch (Exception e)
					{
						_presenter.ReportError(e);
					}
				}

				// Fade-out view
				if (animated && _view is IFadeable asyncPresentable)
				{
					try
					{
						await asyncPresentable.FadeOutAsync();
					}
					catch (Exception e)
					{
						_presenter.ReportError(e);
					}
				}
			}

			// Dispose
			try
			{
				if (_view is INotifyCommand nc)
				{
					nc.Command -= OnCommand;
				}

				if (_view is INotifyDisposed nd)
				{
					nd.Disposed -= OnDismissed;
				}

				_state = State.Disposed;
				_presenter.ControllerFactory.DestroyViewController(_controller);
				_presenter.ViewFactory.ReleaseView(_view);
				_scope?.Dispose();
			}
			catch (Exception e)
			{
				_presenter.ReportError(e);
			}

			// Invoke completion handlers
			try
			{
				SetCompleted();
			}
			catch (Exception e)
			{
				_presenter.ReportError(e);
			}
			finally
			{
				_presenter.OnPresentCompleted(this);
			}
		}

		internal void Update()
		{
			if (_state == State.Active || _state == State.Presented)
			{
				try
				{
					// Call controller update handler (if any).
					if (_controller is IUpdatable ut)
					{
						ut.Update();
					}
				}
				catch (Exception e)
				{
					// NOTE: Do not forward the exception further, just report.
					_presenter.ReportError(e);
				}
			}
		}

		internal bool IsChildOf(PresentResult other)
		{
			Debug.Assert(other != null);

			var p = this;

			while (p != null)
			{
				if (p == other)
				{
					return true;
				}

				p = p.Parent;
			}

			return false;
		}

		protected Task<TResult> GetCompletedTask<TResult>(TResult result)
		{
			if (_cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<TResult>(_cancellationToken);
			}
			else if (_exception != null)
			{
				return Task.FromException<TResult>(_exception);
			}

			return Task.FromResult(result);
		}

		protected void ThrowIfDismissed()
		{
			if (_state >= State.Dismissed)
			{
				throw new InvalidOperationException("Controller has been dismissed.");
			}
		}

		protected void ThrowIfDisposed()
		{
			if (_state == State.Disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		#endregion

		#region virtuals

		protected abstract Task GetPresentTask();
		protected abstract Task GetDismissTask();
		protected abstract void SetCompleted();

		#endregion

		#region IPresentResult

		public Task Task
		{
			get
			{
				if (IsCompleted)
				{
					if (_cancellationToken.IsCancellationRequested)
					{
						return Task.FromCanceled(_cancellationToken);
					}
					else if (_exception != null)
					{
						return Task.FromException(_exception);
					}
					else
					{
						return Task.CompletedTask;
					}
				}

				return GetDismissTask();
			}
		}

		public IViewController Controller => _controller;

		public IView View => _view;

		public bool IsPresented => _state == State.Presented || _state == State.Active;

		public bool IsDismissed => _state >= State.Dismissed;

		#endregion

		#region IPresentContext

		public float PresentTime
		{
			get
			{
				if (_state < State.Presented)
				{
					return 0;
				}

				if (_state >= State.Dismissed)
				{
					return _timer;
				}

				return Time.realtimeSinceStartup - _timer;
			}
		}

		public bool IsActive => _state == State.Active;

		public void Dismiss(Exception e)
		{
			if (_state < State.Dismissed)
			{
				_exception = e;
				_ = DismissAsync(true);
			}
		}

		public void Dismiss(CancellationToken cancellationToken)
		{
			if (_state < State.Dismissed)
			{
				_cancellationToken = cancellationToken;
				_ = DismissAsync(true);
			}
		}

		public void Dismiss()
		{
			if (_state < State.Dismissed)
			{
				_ = DismissAsync(true);
			}
		}

		#endregion

		#region IViewControllerInfo

		public int Id => _id;

		public string DeeplinkId => _deeplinkId;

		public int Tag => _tag;

		public Type ControllerType => _controllerType;

		public Type ViewType => _viewType;

		public Type ResultType => _resultType;

		public Type ArgsType => _argsType;

		#endregion

		#region IPresenter

		public IPresentResult Present(Type controllerType, PresentArgs args)
		{
			ThrowIfDismissed();
			return _presenter.PresentAsync(this, controllerType, args);
		}

		#endregion

		#region ICommandTarget

		public bool InvokeCommand(Command command, Variant args)
		{
			if ((_state == State.Presented || _state == State.Active) && !command.IsNull && _controller is ICommandTarget ct)
			{
				return ct.InvokeCommand(command, args);
			}

			return false;
		}

		#endregion

		#region IServiceProvider

		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(IPresenter) ||
				serviceType == typeof(IPresentContext) ||
				serviceType == typeof(IServiceProvider))
			{
				return this;
			}

			return _serviceProvider.GetService(serviceType);
		}

		#endregion

		#region IEnumerator

		public object Current => null;

		public bool MoveNext()
		{
			return _state != State.Disposed;
		}

		public void Reset()
		{
			throw new NotSupportedException();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			Dismiss();
		}

		#endregion

		#region implementation

		private void OnCommand(object sender, CommandEventArgs e)
		{
			if (e != null)
			{
				InvokeCommand(e.Command, e.Args);
			}
		}

		private void OnDismissed(object sender, EventArgs e)
		{
			if (_state < State.Dismissed)
			{
				_ = DismissAsync(false);
			}
		}

		#endregion
	}
}
