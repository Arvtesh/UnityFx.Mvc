// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.ExceptionServices;
using System.Threading;
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
	internal class PresentResult<TController, TResult> : TaskCompletionSource<TResult>, IPresentContext<TResult>, IPresentResult<TController, TResult>, IPresentable<TController>, IEnumerator where TController : class, IViewController
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
		private readonly Type _controllerType;
		private readonly PresentArgs _presentArgs;
		private readonly PresentOptions _presentOptions;
		private readonly IPresentable _parent;
		private readonly int _id;

		private IServiceProvider _serviceProvider;
		private IDisposable _scope;
		private TController _controller;
		private IView _view;

		private LinkedList<TimerData> _timers;
		private float _timer;

		private List<Exception> _exceptions;
		private State _state;

		#endregion

		#region interface

		public PresentResult(IPresenterInternal presenter, IPresentable parent, Type controllerType, PresentOptions presentOptions, PresentArgs args)
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
			_presentOptions = presentOptions;
			_id = presenter.GetNextId();
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

		public async Task PresentAsync(IViewFactory viewFactory, int index, Transform parent)
		{
			Debug.Assert(viewFactory != null);
			Debug.Assert(_state == State.Initialized);

			try
			{
				_view = await viewFactory.CreateViewAsync(_controllerType, index, _presentOptions, parent);

				if (_state == State.Initialized)
				{
					if (_view is null)
					{
						throw new InvalidOperationException();
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

					if (_controller is IViewControllerEvents c)
					{
						c.OnPresent();
					}

					_state = State.Presented;
				}
				else
				{
					// NOTE: The controller has been dismissed, just dispose the view.
					Debug.Assert(_state == State.Dismissed || _state == State.Disposed);
					_view?.Dispose();
				}
			}
			catch (Exception e)
			{
				LogException(e);
				DismissInternal(default, true);
			}
		}

		public void Update(float frameTime, bool isTop)
		{
			if (_state == State.Active || _state == State.Presented)
			{
				_timer += frameTime;

				UpdateActive(isTop);
				UpdateController(frameTime);
				UpdateTimers(frameTime);
			}
		}

		public void DismissUnsafe()
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
					LogException(e);
				}
				finally
				{
					_state = State.Dismissed;
				}
			}
		}

		public void DisposeUnsafe()
		{
			if (_state != State.Disposed)
			{
				DismissInternal(default, true);
			}
		}

		#endregion

		#region IPresentContext

		public IView View => _view;

		public bool IsDismissed => _state == State.Dismissed || _state == State.Disposed;

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

		public void Dismiss()
		{
			Dismiss(default, false);
		}

		#endregion

		#region IViewControllerInfo

		public int Id => _id;

		public PresentArgs PresentArgs => _presentArgs;

		public PresentOptions PresentOptions => _presentOptions;

		public float Timer => _timer;

		public bool IsActive => _state == State.Active;

		#endregion

		#region IPresenter

		public IPresentResult PresentAsync(Type controllerType, PresentOptions presentOptions, Transform parent, PresentArgs args)
		{
			ThrowIfDisposed();
			return _presenter.PresentAsync(this, controllerType, presentOptions, parent, args);
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

		#region IEnumerator

		public object Current => null;

		public bool MoveNext() => _state != State.Dismissed && _state != State.Disposed;

		public void Reset() => throw new NotSupportedException();

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
						_presenter.Dismiss(this);
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
				if (_state != State.Disposed)
				{
					_state = State.Disposed;

					if (_controller is IDisposable d)
					{
						d.Dispose();
					}

					_view?.Dispose();
					_scope?.Dispose();
				}
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
			}
		}

		private void UpdateActive(bool isTop)
		{
			try
			{
				if (isTop)
				{
					if (_state == State.Presented)
					{
						_state = State.Active;

						if (_controller is IViewControllerEvents c)
						{
							c.OnActivate();
						}
					}
				}
				else if (_state == State.Active)
				{
					_state = State.Presented;

					if (_controller is IViewControllerEvents c)
					{
						c.OnDeactivate();
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		private void UpdateController(float frameTime)
		{
			if (_controller is IViewControllerEvents c)
			{
				try
				{
					c.OnUpdate(frameTime);
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			}
		}

		private void UpdateTimers(float frameTime)
		{
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
						try
						{
							timerData.Callback(timerData.Timer);
						}
						catch (Exception e)
						{
							Debug.LogException(e);
						}

						_timers.Remove(node);
					}

					node = node.Next;
				}
			}
		}

		private void LogException(Exception e)
		{
			Debug.LogException(e);

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
