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
		private IViewController _controller;
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

		protected sealed override void OnStarted()
		{
			try
			{
				base.OnStarted();

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
				base.OnCompleted();

				StateManager.OnPresentCompleted(_controller, this);
			}
			finally
			{
				_controller = null;
				_pushOp = null;
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
			Debug.Assert(_pushOp != null);
			Debug.Assert(_controller != null);

			try
			{
				_pushOp = null;

				if (op.IsCompletedSuccessfully)
				{
					if (_parentState != null && (_args.Options & PresentOptions.DismissCurrentState) != 0)
					{
						_parentState.Dispose();
					}

					_state.OnPresent();

					TrySetResult((T)_controller);
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
