// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Text;
using System.Threading;

namespace UnityFx.AppStates
{
	internal class PopStateOperation : AppStateStackOperation
	{
		#region data

		private readonly AppState _state;

		#endregion

		#region interface

		public AppState State => _state;

		public PopStateOperation(IAppStateOperationOwner owner, AppState state, AsyncCallback asyncCallback, object asyncState)
			: base(owner, AppStateOperationType.Pop, asyncCallback, asyncState, null)
		{
			_state = state;
		}

		#endregion

		#region Object

		public override string ToString()
		{
			return "PopState " + State.Name;
		}

		#endregion
	}
}
