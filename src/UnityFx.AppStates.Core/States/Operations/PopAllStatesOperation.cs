﻿// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Text;
using System.Threading;

namespace UnityFx.AppStates
{
	internal class PopAllStatesOperation : AppStateStackOperation
	{
		#region data
		#endregion

		#region interface

		public PopAllStatesOperation(IAppStateOperationOwner owner, AsyncCallback asyncCallback, object asyncState)
			: base(owner, AppStateOperationType.PopAll, asyncCallback, asyncState, null)
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
