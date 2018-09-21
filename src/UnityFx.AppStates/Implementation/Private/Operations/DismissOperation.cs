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
				base.OnStarted();

				if (_state != null)
				{
					// Remove the state with all its children from the stack.
					DismissStateChildren(_state);

					if (_state.Controller is IPresentable presentable)
					{
						_dismissOp = presentable.DismissAsync(_state.PresentContext);
						_dismissOp.AddCompletionCallback(this);
					}
					else
					{
						SetCompleted();
					}
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
			Debug.Assert(_dismissOp != null);

			try
			{
				if (ProcessNonSuccess(op))
				{
					_dismissOp = null;
					SetCompleted();
				}
			}
			catch (Exception e)
			{
				Fail(e);
			}
		}

		#endregion

		#region implementation

		private void SetCompleted()
		{
			if (_state != null)
			{
				InvokeOnDismiss(_state.Controller);
			}

			TrySetCompleted();
		}

		#endregion
	}
}
