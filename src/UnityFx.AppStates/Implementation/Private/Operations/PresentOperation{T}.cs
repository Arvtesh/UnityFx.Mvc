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
			base.OnStarted();

			try
			{
				if ((_args.Options & PresentOptions.DismissAllStates) != 0)
				{
					DismissAllStates();
				}
				else if ((_args.Options & PresentOptions.DismissCurrentState) != 0 && _parentState != null)
				{
					DismissStateChildren(_parentState);
					InvokeOnDismiss(_parentState.Controller);
				}

				var state = new AppState(StateManager, _parentState, _controllerType, _args);

				_controller = state.Controller;
				_pushOp = state.View.Load();
				_pushOp.AddCompletionCallback(this);
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
				StateManager.OnPresentCompleted(_controller, this);
				base.OnCompleted();
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

		#region Object

		public override string ToString()
		{
			return "Present";
		}

		#endregion

		#region IAsyncContinuation

		public override void Invoke(IAsyncOperation op)
		{
			try
			{
				Debug.Assert(_pushOp != null);
				Debug.Assert(_controller != null);

				if (ProcessNonSuccess(op))
				{
					_pushOp = null;
					InvokeOnViewLoaded(_controller);

					if (_parentState != null && (_args.Options & PresentOptions.DismissCurrentState) != 0)
					{
						_parentState.Dispose();
					}

					InvokeOnPresent(_controller);
					TrySetResult((T)_controller);
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
