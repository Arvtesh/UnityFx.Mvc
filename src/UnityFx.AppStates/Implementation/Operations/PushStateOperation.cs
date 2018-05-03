// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	internal class PushStateOperation : AppStateOperation, IAsyncContinuation
	{
		#region data

		private readonly Type _controllerType;
		private readonly PresentArgs _args;
		private readonly AppState _ownerState;

		private AppState _state;
		private IAsyncOperation _pushOp;
		private IAsyncOperation _transitionOp;

		#endregion

		#region interface

		public PushStateOperation(AppStateService stateManager, AppState ownerState, Type controllerType, PresentArgs args, AsyncCallback asyncCallback, object asyncState)
			: base(stateManager, AppStateOperationType.Push, asyncCallback, asyncState, GetStateDesc(controllerType, args))
		{
			_controllerType = controllerType;
			_args = args;
			_ownerState = ownerState;
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
				_pushOp.AddContinuation(this);
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
					// Make sure the state is popped in case of an error.
					_state?.Pop(this);
				}
			}
			finally
			{
				_state = null;
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
			return "PushState " + GetStateDesc(_controllerType, _args);
		}

		#endregion

		#region IAsyncContinuation

		public void Invoke(IAsyncOperation op, bool inline)
		{
			try
			{
				if (ProcessNonSuccess(op))
				{
					if (_pushOp != null)
					{
						_pushOp = null;
						_state.Controller.InvokeOnViewLoaded();
						_transitionOp = TransitionManager.PlayPushTransition(_state.View);
						_transitionOp.AddContinuation(this);
					}
					else
					{
						TrySetResult(_state, false);
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
