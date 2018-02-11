// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;

namespace UnityFx.AppStates
{
	internal class SetStateOperation : PushStateOperationBase
	{
		#region data

		private readonly AppState _ownerState;

		#endregion

		#region interface

		public AppState OwnerState => _ownerState;

		public SetStateOperation(TraceSource traceSource, AppState ownerState, Type controllerType, object controllerArgs, AsyncCallback asyncCallback, object asyncState)
			: base(traceSource, AppStateOperationType.Set, controllerType, controllerArgs, asyncCallback, asyncState)
		{
			_ownerState = ownerState;
		}

		#endregion
	}
}
