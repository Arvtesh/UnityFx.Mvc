// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	internal class DismissOperation : AppStateOperation, IAsyncContinuation
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
					StateManager.TryDeactivateTopState(this);

					if (_state != null)
					{
						while (States.TryPeek(out var state))
						{
							if (state == _state)
							{
								break;
							}
							else
							{
								state.Controller.InvokeOnDismiss();
								state.Dispose();
							}
						}

						_state.Controller.InvokeOnDismiss();
						_transitionOp = ViewManager.PlayDismissTransition(_state.View);
						_transitionOp.AddContinuation(this);
					}
					else
					{
						while (States.TryPeek(out var state))
						{
							state.Controller.InvokeOnDismiss();
							state.Dispose();
						}

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
				return "PopState " + _state.Id;
			}
			else
			{
				return "PopAll";
			}
		}

		#endregion

		#region implementation
		#endregion
	}
}
