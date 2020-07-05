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
	internal sealed class PresentResult<TResult> : TaskCompletionSource<TResult>, IPresentContext<TResult>, IPresentResult<TResult>, IPresentableProxy, IEnumerator
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

		private struct TimerData
		{
			public float Timeout;
			public float Timer;
			public Action<float> Callback;
		}

		private readonly IPresenterInternal _presenter;
		private readonly IViewControllerFactory _controllerFactory;
		private readonly Type _controllerType;
		private readonly Type _viewType;
		private readonly Type _resultType;
		private readonly ViewControllerFlags _creationFlags;
		private readonly IPresentableProxy _parent;
		private readonly int _id;
		private readonly int _layer;
		private readonly int _tag;
		private readonly string _prefabPath;
		private readonly string _deeplinkId;

		private IServiceProvider _serviceProvider;
		private IDisposable _scope;
		private IViewController _controller;
		private IPresentable _presentEvents;
		private IActivatable _activateEvents;
		private IView _view;

		private LinkedList<TimerData> _timers;
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
			_layer = context.Layer;
			_parent = context.Parent;
			_serviceProvider = context.ServiceProvider;
			_controllerFactory = context.ControllerFactory;
			_controllerType = context.ControllerType;
			_resultType = context.ResultType;
			_viewType = context.ViewType;
			_creationFlags = context.CreationFlags;
			_deeplinkId = MvcUtilities.GetControllerDeeplinkId(_controllerType);
			_prefabPath = string.IsNullOrEmpty(context.ViewResourceId) ? MvcUtilities.GetControllerName(context.ControllerType) : context.ViewResourceId;
		}

		#endregion

		#region IPresentable

		public int Layer => _layer;

		public string PrefabPath => _prefabPath;

		public IPresentableProxy Parent => _parent;

		public bool TryActivate()
		{
			if (_state == State.Presented)
			{
				try
				{
					_activateEvents?.Activate();
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
					_activateEvents?.Deactivate();
				}
				catch (Exception e)
				{
					_presenter.ReportError(e);
				}
			}
		}

		public Task PresentAsyc(IView view, PresentArgs presentArgs)
		{
			Debug.Assert(view != null);
			Debug.Assert(_state == State.Initialized);

			_view = view;
			_scope = _controllerFactory.CreateScope(ref _serviceProvider);
			_controller = _controllerFactory.CreateViewController(_controllerType, this, _view, presentArgs, presentArgs.UserData);
			_activateEvents = _controller as IActivatable;
			_presentEvents = _controller as IPresentable;

			_presentEvents?.Present();
			_view.Disposed += OnDismissed;
			_state = State.Presented;

			return System.Threading.Tasks.Task.CompletedTask;
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
						ut.Update(frameTime);
					}

					// Call timer updates (if any).
					if (_timers != null)
					{
						var node = _timers.First;

						while (node != null)
						{
							var timerData = node.Value;
							timerData.Timer += frameTime;
							node.Value = timerData;

							if (timerData.Timer >= timerData.Timeout)
							{
								_timers.Remove(node);
								timerData.Callback(timerData.Timer);
							}

							node = node.Next;
						}
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

		public void Schedule(Action<float> timerCallback, float timeout)
		{
			ThrowIfDisposed();

			if (timerCallback is null)
			{
				throw new ArgumentNullException(nameof(timerCallback));
			}

			if (timeout < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(timeout));
			}

			if (_timers == null)
			{
				_timers = new LinkedList<TimerData>();
			}

			_timers.AddLast(new TimerData() { Timeout = timeout, Callback = timerCallback });
		}

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

		public ViewControllerFlags CreationFlags => _creationFlags;

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
			if ((_state == State.Presented || _state == State.Active) && _controller is ICommandTarget ct)
			{
				return ct.InvokeCommand(command, args);
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
					_controllerFactory.ReleaseViewController(_controller);
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
				if (_controller is IPresentable c)
				{
					if (_state == State.Active)
					{
						try
						{
							_activateEvents?.Deactivate();
						}
						catch (Exception e)
						{
							_presenter.ReportError(e);
						}

						c.Dismiss();
					}
					else if (_state == State.Presented)
					{
						c.Dismiss();
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

		private void UpdateActive(bool isTop)
		{
			if (isTop)
			{
				if (_state == State.Presented)
				{
					_activateEvents?.Activate();
					_state = State.Active;
				}
			}
			else if (_state == State.Active)
			{
				_state = State.Presented;
				_activateEvents?.Deactivate();
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
