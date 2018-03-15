// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	internal class PopStateOperation : AppStateOperation
	{
		#region data

		private AppState _state;
		private IAsyncOperation _transitionOp;

		#endregion

		#region interface

		public AppState State => _state;

		public PopStateOperation(AppStateManager stateManager, AppState state, AsyncCallback asyncCallback, object asyncState)
			: base(stateManager, AppStateOperationType.Pop, asyncCallback, asyncState, null)
		{
			_state = state;
		}

		#endregion

		#region AsyncResult

		protected override void OnStarted()
		{
			base.OnStarted();

			try
			{
				StateManager.TryDeactivateTopState(this);

				if (_state != null)
				{
					StateManager.PopStates(this, _state);

					_transitionOp = TransitionManager.PlayPopTransition(_state.View);
					_transitionOp.AddCompletionCallback(OnTransitionCompleted);
				}
				else
				{
					while (States.TryPeek(out var state))
					{
						state.Pop(this);
					}

					TrySetCompleted(false);
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
				// Make sure the state is popped. This method can be safely called multiple times.
				_state?.Pop(this);
			}
			finally
			{
				_state = null;
				_transitionOp = null;

				base.OnCompleted();
			}
		}

		#endregion

		#region Object

		public override string ToString()
		{
			if (_state != null)
			{
				return "PopState " + _state.Id;
			}
			else
			{
				return "PopAll";
			}
		}

		#endregion

		#region implementation

		private void OnTransitionCompleted(IAsyncOperation op)
		{
			try
			{
				if (ProcessNonSuccess(op))
				{
					_state?.Pop(this);
					TrySetCompleted(false);
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
