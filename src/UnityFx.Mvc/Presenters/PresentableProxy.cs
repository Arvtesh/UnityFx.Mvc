// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
#if !NET35
using System.Runtime.ExceptionServices;
#endif

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
#if NET35
	internal class PresentableProxy : TreeListNode<PresentableProxy>, IPresentContext, IPresentResult, ICommandTarget
#else
	internal class PresentableProxy : TreeListNode<PresentableProxy>, IPresentContext, IPresentResult, ICommandTarget, CompilerServices.IPresentAwaiter<IPresentable>
#endif
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

#if !NET35

		private Action _presentContinuation;
		private Exception _presentError;

#endif

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

		internal void Present()
		{
			_controller.Dismissed += OnDismissed;
			_controller.LoadViewAsync();

			if (_controller.IsViewLoaded)
			{
				OnPresented();
			}
			else
			{
				_controller.LoadViewCompleted += OnLoadViewCompleted;
			}
		}

		internal bool TryActivate()
		{
			if (_state == State.Presented)
			{
				_state = State.Active;

				if (_controller is IPresentableEvents controllerEvents)
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

				if (_controller is IPresentableEvents controllerEvents)
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
				_controller.Dismiss();
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

		#region IPresentableContext

		public int Id => _id;

		public bool IsActive => _state == State.Active;

		public bool IsModal => (_presentOptions & PresentOptions.Modal) != 0;

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

		public IPresentResult<TController> Present<TController>() where TController : class, IPresentable
		{
			Debug.Assert(!IsDismissed);
			return _presenter.Present<TController>(this, PresentArgs.Default);
		}

		public IPresentResult<TController> Present<TController>(PresentArgs args) where TController : class, IPresentable
		{
			Debug.Assert(!IsDismissed);
			return _presenter.Present<TController>(this, args);
		}

		#endregion

		#region IPresentResult

		public event EventHandler Presented;

		public bool IsPresented => _state == State.Presented || _state == State.Active;

		public IPresentable Controller => _controller;

#if !NET35

		public CompilerServices.IPresentAwaiter<IPresentable> GetAwaiter() => this;

#endif

		#endregion

		#region IDismissable

		public event EventHandler Dismissed { add => _controller.Dismissed += value; remove => _controller.Dismissed -= value; }

		public bool IsDismissed => _controller.IsDismissed;

		public void Dismiss() => _controller.Dismiss();

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

		#region IPresentAwaiter

#if !NET35

		public bool IsCompleted => _state != State.Initialized;

		public IPresentable GetResult()
		{
			if (_presentError != null)
			{
				ExceptionDispatchInfo.Capture(_presentError).Throw();
			}

			return _controller;
		}

#endif

		#endregion

		#region INotifyCompletion

#if !NET35

		public void OnCompleted(Action continuation)
		{
			_presentContinuation += continuation;
		}

		public void UnsafeOnCompleted(Action continuation)
		{
			_presentContinuation += continuation;
		}

#endif

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

		private void OnLoadViewCompleted(object sender, AsyncCompletedEventArgs args)
		{
			_controller.LoadViewCompleted -= OnLoadViewCompleted;

			if (_state == State.Initialized)
			{
				if (args.Error != null || args.Cancelled)
				{
#if !NET35
					_presentError = args.Error ?? new OperationCanceledException();
#endif
					_presenter.Dismissed(this);
					Dispose();
				}
				else
				{
					OnPresented();
					Presented?.Invoke(this, EventArgs.Empty);
				}
			}

#if !NET35
			_presentContinuation?.Invoke();

#endif
		}

		private void OnDismissed(object sender, EventArgs e)
		{
			_controller.Dismissed -= OnDismissed;

			TryDeactivate();

			if (_state != State.Dismissed && _state != State.Disposed)
			{
				_state = State.Dismissed;
				_presenter.Dismissed(this);
			}
		}

		private void OnPresented()
		{
			Debug.Assert(_state == State.Initialized);

			_state = State.Presented;
			_presenter.Presented(this, _presentOptions);
		}

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
