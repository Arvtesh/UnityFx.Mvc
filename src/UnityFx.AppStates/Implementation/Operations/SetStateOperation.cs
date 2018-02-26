// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;

namespace UnityFx.AppStates
{
	internal class SetStateOperation : PushStateOperation
	{
		#region data
		#endregion

		#region interface

		public SetStateOperation(TraceSource traceSource, AppState ownerState, Type controllerType, PushStateArgs args, AsyncCallback asyncCallback, object asyncState)
			: base(traceSource, AppStateOperationType.Set, ownerState, controllerType, args, asyncCallback, asyncState)
		{
		}

		#endregion
	}
}
