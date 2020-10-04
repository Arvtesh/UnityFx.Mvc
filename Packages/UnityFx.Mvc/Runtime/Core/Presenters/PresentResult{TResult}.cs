// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

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
	[Preserve]
	internal sealed class PresentResult<TResult> : TaskCompletionSource<TResult>, IPresentContext<TResult>, IPresentResult<TResult>, IPresenter, IPresentableProxy, IEnumerator
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

		private readonly IPresenterInternal _presenter;
		private readonly IViewControllerFactory _controllerFactory;
		private readonly IViewFactory _viewFactory;
		private readonly Type _controllerType;
		private readonly Type _viewType;
		private readonly Type _resultType;
		private readonly Type _argsType;
		private readonly IPresentableProxy _parent;
		private readonly int _id;
		private readonly int _tag;
		private readonly string _prefabPath;
		private readonly string _deeplinkId;

		private IServiceProvider _serviceProvider;
		private IDisposable _scope;
		private IViewController _controller;
		private IActivateEvents _activateEvents;
		private IView _view;

		private float _timer;

		private List<Exception> _exceptions;
		private State _state;

		#endregion

		#region interface

		public PresentResult(IPresenterInternal presenter, PresentResultArgs context)
		{
			Debug.Assert(presenter != null);
			Debug.Assert(context != null);

			_presenter = presenter;
			_id = context.Id;
			_tag = context.Tag;
			_parent = context.Parent;
			_serviceProvider = context.ServiceProvider;
			_controllerFactory = context.ControllerFactory;
			_controllerType = context.ControllerType;
			_resultType = context.ResultType;
			_argsType = context.ArgsType;
			_viewFactory = context.ViewFactory;
			_viewType = context.ViewType;
			_deeplinkId = PresentUtilities.GetControllerDeeplinkId(_controllerType);
			_prefabPath = string.IsNullOrEmpty(context.ViewResourceId) ? PresentUtilities.GetControllerName(context.ControllerType) : context.ViewResourceId;
		}

		#endregion

		#region IPresentable

		public string PrefabPath => _prefabPath;

		public IPresentableProxy Parent => _parent;

		public bool TryActivate()
		{
			if (_state == State.Presented)
			{
				try
				{
					_activateEvents?.OnActivate();
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

		public void Deactivate()
		{
			if (_state == State.Active)
			{
				try
				{
					_state = State.Presented;
					_activateEvents?.OnDeactivate();
				}
				catch (Exception e)
				{
					_presenter.ReportError(e);
				}
			}
		}

		public async Task PresentAsyc(PresentArgs presentArgs)
		{
			var view = await _viewFactory.CreateViewAsync(_prefabPath, presentArgs.Transform);

			if (view is null)
			{
				throw new PresentException(this, Messages.Format_ViewIsNull());
			}

			if (!_viewType.IsAssignableFrom(view.GetType()))
			{
				throw new PresentException(this, Messages.Format_InvalidViewType(_viewType));
			}

			if (IsDismissed)
			{
				view.Dispose();
				throw new OperationCanceledException();
			}

			_view = view;
			_scope = _controllerFactory.CreateScope(ref _serviceProvider);
			_controller = _controllerFactory.CreateViewController(_controllerType, this, _view, presentArgs, presentArgs.UserData);
			_activateEvents = _controller as IActivateEvents;

			if (view is IAsyncPresentable asyncPresentable)
			{
				await asyncPresentable.PresentAsync();
			}

			if (_controller is IPresentEvents pe)
			{
				pe.OnPresent();
			}

			if (_view is INotifyDisposed nd)
			{
				nd.Disposed += OnDismissed;
			}

			if (_view is INotifyCommand nc)
			{
				nc.Command += OnCommand;
			}

			_state = State.Presented;
		}

		public void Update(float frameTime)
		{
			if (_state == State.Active || _state == State.Presented)
			{
				_timer += frameTime;

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

		public void DismissCancel()
		{
			Dismiss(default, true);
		}

		#endregion

		#region IPresentContext

		public float PresentTime => _timer;

		public bool IsActive => _state == State.Active;

		public void Dismiss(TResult result)
		{
			Dismiss(result, false);
		}

		public void Dismiss(Exception e)
		{
			LogException(e);
			Dismiss(default, e is OperationCanceledException);
		}

		public void Dismiss()
		{
			Dismiss(default, _state == State.Initialized);
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

		#region IViewControllerResultAccess

		public TResult Result => Task.Result;

		#endregion

		#region IPresenter

		public IPresentResult Present(Type controllerType, PresentArgs args)
		{
			ThrowIfDisposed();
			return _presenter.PresentAsync(this, controllerType, args);
		}

		#endregion

		#region IPresentResult

		Task IPresentResult.Task => Task;

		public IViewController Controller => _controller;

		public IView View => _view;

		public bool IsPresented => _state == State.Presented || _state == State.Active;

		public bool IsDismissed => _state == State.Dismissed || _state == State.Disposed;

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
				serviceType == typeof(IPresentContext<TResult>) ||
				serviceType == typeof(IServiceProvider))
			{
				return this;
			}

			return _serviceProvider.GetService(serviceType);
		}

		#endregion

		#region IEnumerator

		public object Current => null;

		public bool MoveNext() => _state != State.Dismissed && _state != State.Disposed;

		public void Reset() => throw new NotSupportedException();

		#endregion

		#region IDisposable

		public void Dispose()
		{
			Dismiss(default, _state == State.Initialized);
		}

		#endregion

		#region implementation

		private void Dismiss(TResult result, bool cancelled)
		{
			if (_state != State.Disposed)
			{
				if (_state == State.Dismissed)
				{
					Dispose(result, cancelled);
				}
				else
				{
					try
					{
						DismissSelf();
						DismissChildren();
					}
					finally
					{
						Dispose(result, cancelled);
					}
				}
			}
		}

		private void Dispose(TResult result, bool cancelled)
		{
			if (_state != State.Disposed)
			{
				_state = State.Disposed;

				try
				{
					_controllerFactory.DestroyViewController(_controller);
					_view?.Dispose();
					_scope?.Dispose();
				}
				catch (Exception e)
				{
					LogException(e);
				}
				finally
				{
					if (_exceptions != null)
					{
						TrySetException(_exceptions);
					}
					else if (cancelled)
					{
						TrySetCanceled();
					}
					else
					{
						TrySetResult(result);
					}

					_presenter.PresentCompleted(this, Task.Exception, cancelled);
				}
			}
			
		}

		private void DismissSelf()
		{
			try
			{
				if (_state == State.Active)
				{
					try
					{
						_activateEvents?.OnDeactivate();
					}
					catch (Exception e)
					{
						_presenter.ReportError(e);
					}
					finally
					{
						_state = State.Presented;
					}
				}

				if (_state == State.Presented)
				{
					if (_controller is IPresentEvents presentEvents)
					{
						presentEvents.OnDismiss();
					}
				}
			}
			catch (Exception e)
			{
				LogException(e);
			}
			finally
			{
				_state = State.Dismissed;
			}
		}

		private void DismissChildren()
		{
			try
			{
				foreach (var child in _presenter.GetChildren(this))
				{
					child.DismissCancel();
				}
			}
			catch (Exception e)
			{
				LogException(e);
			}
			finally
			{
				_state = State.Dismissed;
			}
		}

		private void LogException(Exception e)
		{
			if (!Task.IsCompleted)
			{
				if (_exceptions == null)
				{
					_exceptions = new List<Exception>() { e };
				}
				else
				{
					_exceptions.Add(e);
				}

				_presenter.ReportError(e);
			}
		}

		private void OnCommand(object sender, CommandEventArgs e)
		{
			if (e != null)
			{
				InvokeCommand(e.Command, e.Args);
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
