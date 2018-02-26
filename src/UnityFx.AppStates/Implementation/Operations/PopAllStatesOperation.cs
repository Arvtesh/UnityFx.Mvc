// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;

namespace UnityFx.AppStates
{
	internal class PopAllStatesOperation : AppStateOperation
	{
		#region data
		#endregion

		#region interface

		public PopAllStatesOperation(AppStateManager stateManager, AsyncCallback asyncCallback, object asyncState)
			: base(stateManager, AppStateOperationType.PopAll, asyncCallback, asyncState, null)
		{
		}

		#endregion

		#region Object

		public override string ToString()
		{
			return "PopAll";
		}

		#endregion
	}
}
