// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;

namespace UnityFx.AppStates
{
	internal class PushStateOperation : AppStateOperation
	{
		#region data

		private readonly Type _controllerType;
		private readonly PushStateArgs _args;
		private readonly AppState _ownerState;

		#endregion

		#region interface

		public Type ControllerType => _controllerType;
		public PushStateArgs Args => _args;
		public AppState OwnerState => _ownerState;

		public PushStateOperation(TraceSource traceSource, AppState ownerState, Type controllerType, PushStateArgs args, AsyncCallback asyncCallback, object asyncState)
			: base(traceSource, AppStateOperationType.Push, asyncCallback, asyncState, null)
		{
			_controllerType = controllerType;
			_args = args;
			_ownerState = ownerState;
		}

		public PushStateOperation(TraceSource traceSource, AppStateOperationType opType, AppState ownerState, Type controllerType, PushStateArgs args, AsyncCallback asyncCallback, object asyncState)
			: base(traceSource, opType, asyncCallback, asyncState, null)
		{
			_controllerType = controllerType;
			_args = args;
			_ownerState = ownerState;
		}

		#endregion

		#region Object

		public override string ToString()
		{
			var text = OperationType.ToString() + "State " + AppState.GetStateName(_controllerType);

			if (_args != null)
			{
				text += _args.ToString();
			}

			return text;
		}

		#endregion
	}
}
