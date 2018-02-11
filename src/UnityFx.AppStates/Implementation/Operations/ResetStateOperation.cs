// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;

namespace UnityFx.AppStates
{
	internal class ResetStateOperation : PushStateOperationBase
	{
		#region data
		#endregion

		#region interface

		public ResetStateOperation(TraceSource traceSource, Type controllerType, object controllerArgs, AsyncCallback asyncCallback, object asyncState)
			: base(traceSource, AppStateOperationType.Reset, controllerType, controllerArgs, asyncCallback, asyncState)
		{
		}

		#endregion
	}
}
