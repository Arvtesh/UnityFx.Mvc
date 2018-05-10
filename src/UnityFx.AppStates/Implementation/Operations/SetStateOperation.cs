// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	internal class SetStateOperation : AppStateOperation
	{
		#region data

		private readonly Type _controllerType;
		private readonly PresentArgs _args;
		private readonly bool _isReset;

		private AppState _ownerState;
		private AppState _state;
		private IAsyncOperation _pushOp;
		private IAsyncOperation _transitionOp;

		#endregion

		#region interface

		public SetStateOperation(AppStateService stateManager, AppState ownerState, Type controllerType, PresentArgs args)
			: base(stateManager, "Present", GetStateDesc(controllerType, args))
		{
			_controllerType = controllerType;
			_args = args;
			_ownerState = ownerState;
			_isReset = ownerState == null;
		}

		#endregion

		#region AsyncResult

		protected sealed override void OnStarted()
		{
			base.OnStarted();

			try
			{
				StateManager.TryDeactivateTopState(this);

				if (_ownerState != null)
				{
					StateManager.PopStates(this, _ownerState);
				}
				else if (States.Count > 1)
				{
					_ownerState = States.First;
					StateManager.PopStates(this, _ownerState);
				}
				else if (States.Count == 1)
				{
					_ownerState = States.First;
				}

				_state = new AppState(StateManager, null, _controllerType, PresentOptions.None, _args);
				_pushOp = _state.Push(this);
				_pushOp.AddCompletionCallback(OnStatePushed);
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
					_state?.Pop(this);
				}
			}
			finally
			{
				_ownerState = null;
				_state = null;
				_pushOp = null;
				_transitionOp = null;

				base.OnCompleted();
			}
		}

		#endregion

		#region Object

		public override string ToString()
		{
			if (_isReset)
			{
				return "ResetState " + GetStateDesc(_controllerType, _args);
			}
			else
			{
				return "SetState " + GetStateDesc(_controllerType, _args);
			}
		}

		#endregion

		#region implementation

		private void OnStatePushed(IAsyncOperation op)
		{
			try
			{
				if (ProcessNonSuccess(op))
				{
					_pushOp = null;

					if (_ownerState != null)
					{
						_transitionOp = ViewManager.PlayTransition(_ownerState.View, _state.View);
					}
					else
					{
						_transitionOp = ViewManager.PlayPresentTransition(_state.View);
					}

					_transitionOp.AddCompletionCallback(OnTransitionCompleted);
				}
			}
			catch (Exception e)
			{
				Fail(e);
			}
		}

		private void OnTransitionCompleted(IAsyncOperation op)
		{
			try
			{
				if (ProcessNonSuccess(op))
				{
					_ownerState?.Pop(this);
					_state.Controller.InvokeOnPresent();
					TrySetResult(_state.Controller, false);
				}
			}
			catch (Exception e)
			{
				Fail(e);
			}
		}

		#endregion
	}
}
