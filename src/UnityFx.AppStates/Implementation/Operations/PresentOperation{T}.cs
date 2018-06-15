// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	internal class PresentOperation<T> : AppOperation<T>, IAsyncContinuation where T : AppViewController
	{
		#region data

		private const string _opName = "Present";

		private readonly Type _controllerType;
		private readonly PresentOptions _options;
		private readonly PresentArgs _args;
		private readonly AppState _parentState;
		private readonly AppViewController _parentController;

		private AppViewController _controller;
		private IAsyncOperation _pushOp;
		private IAsyncOperation _transitionOp;

		#endregion

		#region interface

		public PresentOperation(AppStateService stateManager, Type controllerType, PresentOptions options, PresentArgs args)
			: base(stateManager, _opName, GetStateDesc(controllerType, args))
		{
			_controllerType = controllerType;
			_options = options;
			_args = args;
		}

		public PresentOperation(AppStateService stateManager, AppState parentState, Type controllerType, PresentOptions options, PresentArgs args)
			: base(stateManager, _opName, GetStateDesc(controllerType, args))
		{
			_controllerType = controllerType;
			_options = options;
			_args = args;
			_parentState = parentState;
		}

		public PresentOperation(AppStateService stateManager, AppViewController parentController, Type controllerType, PresentOptions options, PresentArgs args)
			: base(stateManager, _opName, GetStateDesc(controllerType, args))
		{
			_controllerType = controllerType;
			_options = options;
			_args = args;
			_parentController = parentController;
		}

		#endregion

		#region AsyncResult

		protected sealed override void OnStarted()
		{
			base.OnStarted();

			try
			{
				if (_parentController != null)
				{
					_controller = _parentController.State.CreateController(_parentController, _controllerType, _options, _args);
				}
				else
				{
					TryDeactivateTopState();

					if ((_options & PresentOptions.DismissAllStates) != 0)
					{
						DismissAllStates();
					}
					else if ((_options & PresentOptions.DismissCurrentState) != 0 && _parentState != null)
					{
						DismissStateChildren(_parentState);
					}

					_controller = new AppState(StateManager, _parentState, _controllerType, _options, _args).Controller;
				}

				_pushOp = _controller.View.Load();
				_pushOp.AddCompletionCallback(this);
			}
			catch (Exception e)
			{
				Fail(e);
			}
		}

		protected override void OnCompleted()
		{
			try
			{
				if (!IsCompletedSuccessfully)
				{
					if (_parentController != null)
					{
						_controller.Dispose();
					}
					else
					{
						_controller.State.Dispose();
					}
				}
			}
			finally
			{
				_controller = null;
				_pushOp = null;
				_transitionOp = null;

				base.OnCompleted();
			}
		}

		protected override void OnCancel()
		{
			_transitionOp?.Cancel();
			base.OnCancel();
		}

		#endregion

		#region Object

		public override string ToString()
		{
			return _opName + ' ' + GetStateDesc(_controllerType, _args);
		}

		#endregion

		#region IAsyncContinuation

		public void Invoke(IAsyncOperation op)
		{
			try
			{
				if (ProcessNonSuccess(op))
				{
					if (_pushOp != null)
					{
						_pushOp = null;
						_controller.InvokeOnViewLoaded();

						if (_parentController != null)
						{
							_transitionOp = ViewManager.PlayPresentTransition(_parentController.View, _controller.View, false);
						}
						else
						{
							_transitionOp = ViewManager.PlayPresentTransition(_parentState?.View, _controller.View, (_options & PresentOptions.DismissCurrentState) != 0);
						}

						_transitionOp.AddCompletionCallback(this);
					}
					else
					{
						if (_parentState != null && (_options & PresentOptions.DismissCurrentState) != 0)
						{
							_parentState.Dispose();
						}

						_controller.InvokeOnPresent();
						TrySetResult(_controller as T);
					}
				}
			}
			catch (Exception e)
			{
				Fail(e);
			}
		}

		#endregion

		#region implementation
		#endregion
	}
}
