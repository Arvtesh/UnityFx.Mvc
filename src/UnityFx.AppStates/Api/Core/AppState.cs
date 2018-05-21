// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Enumerates state presentation modes.
	/// </summary>
	public enum AppStatePresentationMode
	{
		/// <summary>
		/// Default (non-modal popup).
		/// </summary>
		Default,

		/// <summary>
		/// Exclusive (covers whole screen).
		/// </summary>
		Exclusive,

		/// <summary>
		/// Modal popup (appears on top of non-modal views).
		/// </summary>
		Modal,
	}

	/// <summary>
	/// A generic application state.
	/// </summary>
	/// <remarks>
	/// By design an application flow is a sequence of state switches. A state may represent a single screen,
	/// a dialog or a menu. States are supposed to be as independent as possible. Only one state may be active
	/// (i.e. process user input) at time, but unlimited number of states may exist (be rendered on the screen
	/// and execute their code) at the same time.
	/// </remarks>
	/// <seealso href="http://gameprogrammingpatterns.com/state.html"/>
	/// <seealso href="https://en.wikipedia.org/wiki/State_pattern"/>
	public class AppState : LinkedListNode<AppState>, IAppViewControllerContext, IPresenter, IDismissable
	{
		#region data

		private enum AppStateState
		{
			Created,
			Pushed,
			Popped,
			Disposed
		}

		private readonly TraceSource _console;
		private readonly AppStateService _stateManager;
		private readonly AppViewController _controller;

		private AppStateState _state;
		private bool _isActive;

		private AppViewController _tmpController;
		private PresentOptions _tmpControllerOptions;
		private PresentArgs _tmpControllerArgs;

		#endregion

		#region interface

		internal IAppViewService ViewManager => _stateManager.ViewManager;
		internal IAppViewControllerFactory ControllerFactory => _stateManager.ControllerFactory;

		internal AppState(AppStateService stateManager, AppState parentState, Type controllerType, PresentOptions options, PresentArgs args)
			: base(parentState)
		{
			Debug.Assert(stateManager != null);
			Debug.Assert(controllerType != null);

			_tmpControllerOptions = options;
			_tmpControllerArgs = args;

			_stateManager = stateManager;
			_console = stateManager.TraceSource;
			_controller = stateManager.ControllerFactory.CreateController(controllerType, this);
			_stateManager.States.Add(this);
		}

		internal void Pop(IAppOperationInfo op)
		{
			if (_state == AppStateState.Pushed)
			{
				_console.TraceData(TraceEventType.Verbose, op.OperationId, "PopState " + _controller.Id);
				_stateManager.States.Remove(this);
				_state = AppStateState.Popped;
			}

			Dispose();
		}

		internal void Activate(IAppOperationInfo op)
		{
			Debug.Assert(_state == AppStateState.Pushed);

			if (!_isActive && (Parent == null || Parent.IsActive))
			{
				_console.TraceEvent(TraceEventType.Verbose, op.OperationId, "ActivateState " + _controller.Id);

				_isActive = true;
				_controller.InvokeOnActivate();
			}
		}

		internal void Deactivate(IAppOperationInfo op)
		{
			Debug.Assert(_state == AppStateState.Pushed);

			if (_isActive)
			{
				_console.TraceData(TraceEventType.Verbose, op.OperationId, "DeactivateState " + _controller.Id);

				try
				{
					_controller.InvokeOnDeactivate();
				}
				finally
				{
					_isActive = false;
				}
			}
		}

		internal AppView GetPrevView()
		{
			return Prev?.Controller.GetTopView();
		}

		internal AppViewController CreateController(AppViewController c, Type controllerType, PresentOptions options, PresentArgs args)
		{
			ThrowIfDisposed();

			_tmpControllerArgs = args;
			_tmpControllerOptions = options;
			_tmpController = c;

			try
			{
				var controller = _stateManager.ControllerFactory.CreateController(controllerType, this);
				c.AddChildController(controller);
				return controller;
			}
			finally
			{
				_tmpControllerArgs = null;
				_tmpControllerOptions = PresentOptions.None;
				_tmpController = null;
			}
		}

		#endregion

		#region IAppState

		/// <summary>
		/// Gets the state type identifier.
		/// </summary>
		public string Id => _controller.Id;

		/// <summary>
		/// Gets the state path.
		/// </summary>
		public string Path
		{
			get
			{
				var localPath = '/' + _controller.Id;
				return Parent?.Path + localPath ?? localPath;
			}
		}

		/// <summary>
		/// Gets a view instance attached to the state.
		/// </summary>
		public AppView View => _controller.View;

		/// <summary>
		/// Gets a controller attached to the state.
		/// </summary>
		public AppViewController Controller => _controller;

		/// <summary>
		/// Gets a value indicating whether the state is active.
		/// </summary>
		public bool IsActive => _isActive;

		/// <summary>
		/// Gets a collection of the state's children.
		/// </summary>
		public IReadOnlyCollection<AppState> Substates => _stateManager.States.GetChildren(this);

		/// <summary>
		/// Throws <see cref="ObjectDisposedException"/> if the instance is disposed.
		/// </summary>
		protected void ThrowIfDisposed()
		{
			if (_state == AppStateState.Disposed)
			{
				throw new ObjectDisposedException(_controller.Id);
			}
		}

		#endregion

		#region IAppViewControllerContext

		/// <inheritdoc/>
		PresentOptions IAppViewControllerContext.PresentOptions => _tmpControllerOptions;

		/// <inheritdoc/>
		PresentArgs IAppViewControllerContext.PresentArgs => _tmpControllerArgs;

		/// <inheritdoc/>
		AppViewController IAppViewControllerContext.ParentController => _tmpController;

		/// <inheritdoc/>
		AppState IAppViewControllerContext.ParentState => this;

		/// <inheritdoc/>
		AppView IAppViewControllerContext.CreateView(AppViewController c)
		{
			ThrowIfDisposed();

			if (_tmpController == null)
			{
				return _stateManager.ViewManager.CreateView(c.Id, GetPrevView(), AppViewOptions.None);
			}
			else if ((c.CreationOptions & AppViewControllerOptions.ReuseParentView) != 0)
			{
				return _stateManager.ViewManager.CreateChildView(c.Id, _tmpController.View, AppViewOptions.None);
			}
			else
			{
				return _stateManager.ViewManager.CreateView(c.Id, _tmpController.GetTopView(), AppViewOptions.None);
			}
		}

		/// <inheritdoc/>
		IAsyncOperation<AppViewController> IAppViewControllerContext.PresentAsync(AppViewController parentController, Type controllerType, PresentOptions options, PresentArgs args)
		{
			ThrowIfDisposed();

			return _stateManager.PresentAsync(parentController, controllerType, options, args);
		}

		/// <inheritdoc/>
		IAsyncOperation IAppViewControllerContext.DismissAsync(AppViewController controller)
		{
			ThrowIfDisposed();

			return _stateManager.DismissAsync(controller);
		}

		#endregion

		#region IPresenter

		/// <inheritdoc/>
		public IAsyncOperation<AppViewController> PresentAsync(Type controllerType, PresentOptions options, PresentArgs args)
		{
			ThrowIfDisposed();

			return _stateManager.PresentAsync(this, controllerType, options, args);
		}

		/// <inheritdoc/>
		public IAsyncOperation<AppViewController> PresentAsync(Type controllerType, PresentArgs args)
		{
			ThrowIfDisposed();

			return _stateManager.PresentAsync(this, controllerType, PresentOptions.None, args);
		}

		/// <inheritdoc/>
		public IAsyncOperation<TController> PresentAsync<TController>(PresentOptions options, PresentArgs args) where TController : AppViewController
		{
			return PresentAsync(typeof(TController), options, args) as IAsyncOperation<TController>;
		}

		/// <inheritdoc/>
		public IAsyncOperation<TController> PresentAsync<TController>(PresentArgs args) where TController : AppViewController
		{
			return PresentAsync(typeof(TController), args) as IAsyncOperation<TController>;
		}

		#endregion

		#region IDismissable

		/// <inheritdoc/>
		public IAsyncOperation DismissAsync()
		{
			ThrowIfDisposed();

			return _stateManager.DismissAsync(this);
		}

		#endregion

		#region IDisposable

		/// <inheritdoc/>
		public void Dispose()
		{
			if (_state != AppStateState.Disposed)
			{
				_state = AppStateState.Disposed;
				_stateManager.States.Remove(this);
				_controller.Dispose();
			}
		}

		#endregion

		#region implementation
		#endregion
	}
}
