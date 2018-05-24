// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	internal class DismissOperation : AppOperation, IAsyncContinuation
	{
		#region data

		private AppViewController _controller;
		private AppState _state;
		private IAsyncOperation _transitionOp;

		#endregion

		#region interface

		public DismissOperation(AppStateService stateManager, AppState state)
			: base(stateManager, "Dismiss", null)
		{
			_state = state;
		}

		public DismissOperation(AppStateService stateManager, AppViewController controller)
			: base(stateManager, "Dismiss", null)
		{
			_controller = controller;
		}

		#endregion

		#region AsyncResult

		protected override void OnStarted()
		{
			base.OnStarted();

			try
			{
				if (_controller != null)
				{
					_controller.InvokeOnDismiss();
					_transitionOp = ViewManager.PlayDismissTransition(_controller.View);
					_transitionOp.AddContinuation(this);
				}
				else
				{
					TryDeactivateTopState();

					if (_state != null)
					{
						DismissStateChildren(_state);
						_state.DismissInternal(this);

						_transitionOp = ViewManager.PlayDismissTransition(_state.View);
						_transitionOp.AddContinuation(this);
					}
					else
					{
						DismissAllStates();
						TrySetCompleted(false);
					}
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
				if (_controller != null)
				{
					_controller.Dispose();
				}
				else
				{
					_state?.Dispose();
				}
			}
			finally
			{
				_state = null;
				_controller = null;
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

		#region IAsyncContinuation

		public void Invoke(IAsyncOperation op, bool inline)
		{
			try
			{
				if (ProcessNonSuccess(op))
				{
					TrySetCompleted(inline);
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
			if (_state != null)
			{
				return "DismissState " + _state.TypeId;
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
