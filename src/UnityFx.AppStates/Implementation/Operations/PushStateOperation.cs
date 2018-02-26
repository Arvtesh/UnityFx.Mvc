// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	internal class PushStateOperation : AppStateOperation
	{
		#region data

		private readonly Type _controllerType;
		private readonly PushStateArgs _args;
		private readonly AppState _ownerState;

		private AppState _state;

		#endregion

		#region interface

		public PushStateOperation(AppStateManager stateManager, AppState ownerState, Type controllerType, PushStateArgs args, AsyncCallback asyncCallback, object asyncState)
			: base(stateManager, AppStateOperationType.Push, asyncCallback, asyncState, null)
		{
			_controllerType = controllerType;
			_args = args;
			_ownerState = ownerState;
		}

		#endregion

		#region AsyncResult

		protected sealed override void OnStarted()
		{
			base.OnStarted();

			try
			{
				StateManager.TryDeactivateTopState();

				_state = new AppState(StateManager, _ownerState, _controllerType, _args);
				_state.Push().AddCompletionCallback(OnStatePushed);
			}
			catch (Exception e)
			{
				TrySetException(e, false);

				if (_state != null)
				{
					_state.PopAndDispose();
					_state = null;
				}
			}
		}

		#endregion

		#region Object

		public override string ToString()
		{
			var text = "PushState " + AppState.GetStateName(_controllerType);

			if (_args != null)
			{
				text += _args.ToString();
			}

			return text;
		}

		#endregion

		#region implementation

		private void OnStatePushed(IAsyncOperation op)
		{
			if (op.IsCompletedSuccessfully)
			{
				TrySetResult(_state, false);
			}
			else if (op.IsFaulted)
			{
				TrySetException(op.Exception, false);
			}
			else
			{
				TrySetCanceled(false);
			}

			StateManager.TryActivateTopState();
		}

		#endregion
	}
}
