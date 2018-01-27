// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Text;
using System.Threading;

namespace UnityFx.AppStates
{
	internal class PushStateOperation : PushStateOperationBase
	{
		#region data

		private readonly AppState _ownerState;

		#endregion

		#region interface

		public AppState OwnerState => _ownerState;

		public PushStateOperation(IAppStateOperationOwner owner, AppState ownerState, Type controllerType, object controllerArgs, AsyncCallback asyncCallback, object asyncState)
			: base(owner, AppStateOperationType.Push, controllerType, controllerArgs, asyncCallback, asyncState)
		{
			_ownerState = ownerState;
		}

		#endregion
	}
}
