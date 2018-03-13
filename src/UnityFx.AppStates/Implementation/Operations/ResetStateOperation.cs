// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;

namespace UnityFx.AppStates
{
	internal class ResetStateOperation : AppStateOperation
	{
		#region data

		private readonly Type _controllerType;
		private readonly PushStateArgs _args;

		#endregion

		#region interface

		public ResetStateOperation(AppStateManager stateManager, Type controllerType, PushStateArgs args, AsyncCallback asyncCallback, object asyncState)
			: base(stateManager, AppStateOperationType.Reset, asyncCallback, asyncState, GetStateDesc(controllerType, args))
		{
			_controllerType = controllerType;
			_args = args;
		}

		#endregion

		#region AsyncResult

		protected sealed override void OnStarted()
		{
			base.OnStarted();

			// TODO
		}

		#endregion

		#region Object

		public override string ToString()
		{
			var text = "ResetState " + AppState.GetStateName(_controllerType);

			if (_args != null)
			{
				text += _args.ToString();
			}

			return text;
		}

		#endregion
	}
}
