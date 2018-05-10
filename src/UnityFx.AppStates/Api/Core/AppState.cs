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
	public class AppState : IDisposable
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
		private readonly AppState _parentState;

		private AppStateState _state;
		private bool _isActive;

		#endregion

		#region interface

		internal bool IsPushed => _state == AppStateState.Pushed;
		internal IAppViewManager ViewManager => _stateManager.ViewManager;
		internal IAppControllerFactory ControllerFactory => _stateManager.ControllerFactory;

		internal AppState PrevState { get; set; }
		internal AppState NextState { get; set; }

		internal AppViewController TmpController { get; set; }
		internal PresentOptions TmpControllerOptions { get; set; }
		internal PresentArgs TmpControllerArgs { get; set; }

		internal AppState(AppStateService stateManager, AppState parentState, Type controllerType, PresentOptions options, PresentArgs args)
		{
			Debug.Assert(stateManager != null);
			Debug.Assert(controllerType != null);

			TmpControllerOptions = options;
			TmpControllerArgs = args;

			_stateManager = stateManager;
			_parentState = parentState;
			_console = stateManager.TraceSource;
			_controller = stateManager.ControllerFactory.CreateController(controllerType, this);
		}

		internal IAsyncOperation Push(IAppStateOperationInfo op)
		{
			Debug.Assert(_state == AppStateState.Created);

			_console.TraceData(TraceEventType.Verbose, op.OperationId, "PushState " + _controller.Id);
			_stateManager.States.Add(this);
			_state = AppStateState.Pushed;

			return _controller.View.Load();
		}

		internal void Pop(IAppStateOperationInfo op)
		{
			if (_state == AppStateState.Pushed)
			{
				_console.TraceData(TraceEventType.Verbose, op.OperationId, "PopState " + _controller.Id);
				_stateManager.States.Remove(this);
				_state = AppStateState.Popped;
			}

			Dispose();
		}

		internal void Activate(IAppStateOperationInfo op)
		{
			Debug.Assert(_state == AppStateState.Pushed);

			if (!_isActive && (_parentState == null || _parentState.IsActive))
			{
				_console.TraceEvent(TraceEventType.Verbose, op.OperationId, "ActivateState " + _controller.Id);

				_isActive = true;
				_controller.InvokeOnActivate();
			}
		}

		internal void Deactivate(IAppStateOperationInfo op)
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
			return PrevState?.Controller.GetTopView();
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
				return _parentState?.Path + localPath ?? localPath;
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
		/// Gets a parent state.
		/// </summary>
		public AppState Parent => _parentState;

		/// <summary>
		/// Gets a collection of the state's children.
		/// </summary>
		public IReadOnlyCollection<AppState> Substates
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Removes the state from the stack.
		/// </summary>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ObjectDisposedException">Thrown if the state is disposed.</exception>
		public IAsyncOperation DismissAsync()
		{
			ThrowIfDisposed();
			return _stateManager.PopStateAsync(this);
		}

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
