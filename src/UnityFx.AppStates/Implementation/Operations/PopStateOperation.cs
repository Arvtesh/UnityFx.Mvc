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
				StateManager.PopStateDependencies(this, _state);

				_transitionOp = TransitionManager.PlayPopTransition(_state.View);
				_transitionOp.AddCompletionCallback(OnTransitionCompleted);
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

			_state = null;
			_transitionOp = null;
		}

		#endregion

		#region Object

		public override string ToString()
		{
			return "PopState " + State.Id;
		}

		#endregion

		#region implementation

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
					_state.Pop(this);

					StateManager.TryActivateTopState(this);
					TrySetCompleted(false);
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
