// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;

namespace UnityFx.AppStates
{
	internal class PopStateOperation : AppStateOperation
	{
		#region data

		private readonly AppState _state;

		#endregion

		#region interface

		public AppState State => _state;

		public PopStateOperation(AppStateManager stateManager, AppState state, AsyncCallback asyncCallback, object asyncState)
			: base(stateManager, AppStateOperationType.Pop, asyncCallback, asyncState, null)
		{
			_state = state;
		}

		#endregion

		#region AsyncResult

		protected override void OnStarted()
		{
			base.OnStarted();
		}

		#endregion

		#region Object

		public override string ToString()
		{
			return "PopState " + State.Id;
		}

		#endregion
	}
}
