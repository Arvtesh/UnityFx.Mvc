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
	internal sealed class PresentResult<TController, TResult> : TaskCompletionSource<TResult>, IPresentContext<TResult>, IPresentResult<TResult>, IPresentResultOf<TController, TResult>, IPresentResultOf<TController>, IPresentable, IEnumerator where TController : class, IViewController
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
		private readonly PresentArgs _presentArgs;
		private readonly PresentOptions _presentOptions;
		private readonly IPresentable _parent;
		private readonly int _id;
		private readonly int _layer;
		private readonly int _tag;
		private readonly string _prefabPath;
		private readonly string _deeplinkId;

		private IServiceProvider _serviceProvider;
		private IDisposable _scope;
		private TController _controller;
		private IViewControllerEvents _controllerEvents;
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
			_presentArgs = context.PresentArgs;
			_presentOptions = context.PresentOptions;
			_deeplinkId = GetDeeplinkId(_controllerType);
			_prefabPath = string.IsNullOrEmpty(context.PrefabPath) ? GetDefaultPrefabName(_controllerType) : context.PrefabPath;
		}

		#endregion

		#region IPresentable

		public int Layer => _layer;

		public string PrefabPath => _prefabPath;

		public IPresentable Parent => _parent;

		IViewController IPresentable.Controller => _controller;

		public bool TryActivate()
		{
			if (_state == State.Presented)
			{
				_state = State.Active;
				_controllerEvents?.OnActivate();
				return true;
			}

			return false;
		}

		public bool TryDeactivate()
		{
			if (_state == State.Active)
			{
				_state = State.Presented;
				_controllerEvents?.OnDeactivate();
				return true;
			}

			return false;
		}

		public void CreateController(IView view)
		{
			Debug.Assert(view != null);
			Debug.Assert(_state == State.Initialized);

			try
			{
				_view = view;
				_scope = _controllerFactory.CreateScope(ref _serviceProvider);
				_controller = (TController)_controllerFactory.CreateViewController(_controllerType, this, _presentArgs, _view);
				_controllerEvents = _controller as IViewControllerEvents;
				_controllerEvents?.OnPresent();
				_view.Disposed += OnDismissed;
				_state = State.Presented;
			}
			catch (Exception e)
			{
				LogException(e);
				Dispose(default, true);
				throw;
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

		public void DismissCancel()
		{
			Dismiss(default, true);
		}

		#endregion

		#region IPresentContext

		public IView View => _view;

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

		public PresentArgs PresentArgs => _presentArgs;

		public PresentOptions PresentOptions => _presentOptions;

		public bool IsDismissed => _state == State.Dismissed || _state == State.Disposed;

		#endregion

		#region IPresenter

		public IPresentResult Present(Type controllerType, PresentArgs args, PresentOptions presentOptions, Transform parent)
		{
			ThrowIfDisposed();
			return _presenter.PresentAsync(this, controllerType, presentOptions, parent, args);
		}

		#endregion

		#region IPresentResult

		Task IPresentResult.Task => Task;

		public TController Controller => _controller;

		public TResult Result => Task.Result;

		#endregion

		#region ICommandTarget

		public bool InvokeCommand<TCommand>(TCommand command)
		{
			if ((_state == State.Presented || _state == State.Active) && _controller is ICommandTarget ct)
			{
				return ct.InvokeCommand(command);
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
			try
			{
				if (_state != State.Disposed)
				{
					_state = State.Disposed;
					_controllerFactory.ReleaseViewController(_controller);
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

		private void DismissSelf()
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
					_state = State.Active;
					_controllerEvents?.OnActivate();
				}
			}
			else if (_state == State.Active)
			{
				_state = State.Presented;
				_controllerEvents?.OnDeactivate();
			}
		}

		private void UpdateController(float frameTime)
		{
			if (_controller is IUpdateTarget ut)
			{
				ut.Update(frameTime);
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
						_timers.Remove(node);
						timerData.Callback(timerData.Timer);
					}

					node = node.Next;
				}
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

		private static string GetDeeplinkId(Type controllerType)
		{
			var deeplinkId = controllerType.Name.ToLower();

			if (deeplinkId.EndsWith("controller"))
			{
				deeplinkId = deeplinkId.Substring(0, deeplinkId.Length - 10);
			}

			return deeplinkId;
		}

		private static string GetDefaultPrefabName(Type controllerType)
		{
			var deeplinkId = controllerType.Name;

			if (deeplinkId.EndsWith("Controller"))
			{
				deeplinkId = deeplinkId.Substring(0, deeplinkId.Length - 10);
			}

			return deeplinkId;
		}

		#endregion
	}
}
