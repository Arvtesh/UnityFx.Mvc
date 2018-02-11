// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;

namespace UnityFx.AppStates
{
	internal class PushStateOperationBase : AppStateStackOperation
	{
		#region data

		private readonly Type _controllerType;
		private readonly object _controllerArgs;

		#endregion

		#region interface

		public Type ControllerType => _controllerType;

		public object ControllerArgs => _controllerArgs;

		public PushStateOperationBase(TraceSource traceSource, AppStateOperationType opType, Type controllerType, object controllerArgs, AsyncCallback asyncCallback, object asyncState)
			: base(traceSource, opType, asyncCallback, asyncState, null)
		{
			_controllerType = controllerType;
			_controllerArgs = controllerArgs;
		}

		#endregion

		#region Object

		public override string ToString()
		{
			var text = OperationType.ToString() + "State " + AppState.GetStateName(_controllerType);

			if (_controllerArgs != null)
			{
				text += ", ";
				text += _controllerArgs.ToString();
			}

			return text;
		}

		#endregion
	}
}
