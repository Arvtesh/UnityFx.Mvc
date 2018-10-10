// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	internal class PresentOperation<T> : AppStateOperation<T> where T : class, IViewController
	{
		#region data

		private readonly Type _controllerType;
		private readonly PresentArgs _args;
		private readonly AppState _parentState;

		private AppState _state;
		private IAsyncOperation _pushOp;

		#endregion

		#region interface

		public PresentOperation(AppStateService stateManager, AppState parentState, Type controllerType, PresentArgs args)
			: base(stateManager, args.Data, GetStateDesc(controllerType, args))
		{
			Debug.Assert(controllerType != null);
			Debug.Assert(args != null);

			_controllerType = controllerType;
			_args = args;
			_parentState = parentState;
		}

		#endregion

		#region AsyncResult

		protected override void OnStarted()
		{
			try
			{
				TraceStart();
				StateManager.InvokePresentStarted(_controllerType, _args, this);

				if ((_args.Options & PresentOptions.DismissAllStates) != 0)
				{
					DismissAllStates();
				}
				else if ((_args.Options & PresentOptions.DismissCurrentState) != 0 && _parentState != null)
				{
					_parentState.DismissChildStates();
					_parentState.OnDismiss();
				}

				_state = new AppState(StateManager, _parentState, _controllerType, _args);
				_pushOp = _state.PresentAsync(default(IPresentContext));
				_pushOp.AddCompletionCallback(this);
			}
			catch (Exception e)
			{
				TrySetException(e);
			}
		}

		protected override void OnCompleted()
		{
			try
			{
				// This should not throw.
				StateManager.InvokePresentCompleted(_state, _state?.Controller, this);

				// Make sure the state is disposed on operation failure.
				if (!IsCompletedSuccessfully)
				{
					_state?.Dispose();
				}
			}
			finally
			{
				TraceStop(Status);
			}
		}

		protected override void OnCancel()
		{
			TrySetCanceled();
		}

		#endregion

		#region IAsyncContinuation

		public override void Invoke(IAsyncOperation op)
		{
			Debug.Assert(_pushOp != null);
			Debug.Assert(_state != null);

			try
			{
				_pushOp = null;

				if (!op.CompletedSynchronously)
				{
					SetCompletedAsynchronously();
				}

				if (op.IsCompletedSuccessfully)
				{
					// Make sure parent state is disposed.
					if (_parentState != null && (_args.Options & PresentOptions.DismissCurrentState) != 0)
					{
						_parentState.Dispose();
					}

					// Present the new state. If this thows the operation should fail as well.
					_state.OnPresent();

					TrySetResult((T)_state.Controller);
				}
				else
				{
					TrySetException(op.Exception);
				}
			}
			catch (Exception e)
			{
				TrySetException(e);
			}
		}

		#endregion

		#region Object

		public override string ToString()
		{
			return "Present";
		}

		#endregion

		#region implementation
		#endregion
	}
}
