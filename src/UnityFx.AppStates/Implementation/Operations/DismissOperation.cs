// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	internal class DismissOperation : AppStateOperation<object>
	{
		#region data

		private AppState _state;
		private IAsyncOperation _dismissOp;

		#endregion

		#region interface

		public DismissOperation(AppStateService stateManager, AppState state, object asyncState)
			: base(stateManager, asyncState, null)
		{
			_state = state;
		}

		#endregion

		#region AsyncResult

		protected override void OnStarted()
		{
			try
			{
				TraceStart();
				StateManager.InvokeDismissStarted(_state, _state.Controller, this);

				if (_state != null)
				{
					_dismissOp = _state.DismissAsync(null);
					_dismissOp.AddCompletionCallback(this);
				}
				else
				{
					// Remove all states from the stack.
					DismissAllStates();
					TrySetCompleted();
				}
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
				StateManager.InvokeDismissCompleted(_state, _state.Controller, this);

				// The state should be disposed in any case.
				_state?.Dispose();
				_state = null;
			}
			finally
			{
				TraceStop(Status);
			}
		}

		#endregion

		#region IAsyncContinuation

		public override void Invoke(IAsyncOperation op)
		{
			Debug.Assert(op != null);
			Debug.Assert(_dismissOp != null);

			try
			{
				_dismissOp = null;

				if (!op.CompletedSynchronously)
				{
					SetCompletedAsynchronously();
				}

				if (op.IsCompletedSuccessfully)
				{
					TrySetCompleted();
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
			if (_state != null)
			{
				return "Dismiss_" + _state.Name;
			}
			else
			{
				return "DismissAll";
			}
		}

		#endregion

		#region implementation
		#endregion
	}
}
