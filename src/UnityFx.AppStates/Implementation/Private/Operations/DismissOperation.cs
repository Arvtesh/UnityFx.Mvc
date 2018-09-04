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

		private IAppState _state;

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
				base.OnStarted();

				if (_state != null)
				{
					// Remove the state with all its children from the stack.
					DismissStateChildren(_state);
					InvokeOnDismiss(_state.Controller);
				}
				else
				{
					// Remove all states from the stack.
					DismissAllStates();
				}

				TrySetCompleted();
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
				if (_state != null)
				{
					StateManager.OnDismissCompleted(_state, this);

					_state.Dispose();
					_state = null;
				}
				else
				{
					StateManager.OnDismissCompleted(null, this);
				}

				base.OnCompleted();
			}
			finally
			{
				_state = null;
			}
		}

		protected override void OnCancel()
		{
			base.OnCancel();
		}

		#endregion

		#region IAsyncContinuation

		public override void Invoke(IAsyncOperation op)
		{
			try
			{
				if (ProcessNonSuccess(op))
				{
					TrySetCompleted();
				}
			}
			catch (Exception e)
			{
				Fail(e);
			}
		}

		#endregion

		#region Object

		public override string ToString()
		{
			return "Dismiss";
		}

		#endregion

		#region implementation
		#endregion
	}
}
