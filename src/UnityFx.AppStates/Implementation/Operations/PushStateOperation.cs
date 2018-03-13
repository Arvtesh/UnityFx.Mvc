// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	internal class PushStateOperation : AppStateOperation
	{
		#region data

		private readonly Type _controllerType;
		private readonly PushStateArgs _args;
		private readonly AppState _ownerState;

		private AppState _state;
		private IAsyncOperation _pushOp;
		private IAsyncOperation _transitionOp;

		#endregion

		#region interface

		public PushStateOperation(AppStateManager stateManager, AppState ownerState, Type controllerType, PushStateArgs args, AsyncCallback asyncCallback, object asyncState)
			: base(stateManager, AppStateOperationType.Push, asyncCallback, asyncState, GetStateDesc(controllerType, args))
		{
			_controllerType = controllerType;
			_args = args;
			_ownerState = ownerState;
		}

		#endregion

		#region AppStateOperation

		internal override void Cancel()
		{
			if (_transitionOp != null)
			{
				TransitionManager.CancelTransition(_transitionOp);
			}

			base.Cancel();
		}

		#endregion

		#region AsyncResult

		protected sealed override void OnStarted()
		{
			base.OnStarted();

			try
			{
				StateManager.TryDeactivateTopState(this);

				_state = new AppState(StateManager, _ownerState, _controllerType, _args);
				_pushOp = _state.Push(this);
				_pushOp.AddCompletionCallback(OnStatePushed);
			}
			catch (Exception e)
			{
				TraceException(e);
				TrySetException(e, false);
			}
		}

		protected override void OnCompleted()
		{
			base.OnCompleted();

			if (!IsCompletedSuccessfully)
			{
				if (_state != null)
				{
					_state.Pop(this);
					_state = null;
				}

				_pushOp = null;
				_transitionOp = null;
			}
		}

		#endregion

		#region Object

		public override string ToString()
		{
			return "PushState " + GetStateDesc(_controllerType, _args);
		}

		#endregion

		#region implementation

		private void OnStatePushed(IAsyncOperation op)
		{
			try
			{
				if (op.IsFaulted)
				{
					TrySetException(op.Exception, false);
				}
				else if (op.IsCanceled)
				{
					TrySetCanceled(false);
				}
				else
				{
					_transitionOp = TransitionManager.PlayPushTransition(_state.View);
					_transitionOp.AddCompletionCallback(OnTransitionCompleted);
				}
			}
			catch (Exception e)
			{
				TraceException(e);
				TrySetException(e, false);
			}
		}

		private void OnTransitionCompleted(IAsyncOperation op)
		{
			try
			{
				if (op.IsFaulted)
				{
					TrySetException(op.Exception, false);
				}
				else if (op.IsCanceled)
				{
					TrySetCanceled(false);
				}
				else
				{
					StateManager.TryActivateTopState(this);
					TrySetResult(_state, false);
				}
			}
			catch (Exception e)
			{
				TraceException(e);
				TrySetException(e, false);
			}
		}

		#endregion
	}
}
