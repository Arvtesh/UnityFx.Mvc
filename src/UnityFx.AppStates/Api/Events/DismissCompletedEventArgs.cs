// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Event arguments for <see cref="IAppStateService.DismissCompleted"/>.
	/// </summary>
	public class DismissCompletedEventArgs : AsyncCompletedEventArgs
	{
		#region data

		private readonly int _id;
		private readonly IAppState _state;
		private readonly IViewController _controller;

		#endregion

		#region interface

		/// <summary>
		/// Gets identifier of the dismiss operation.
		/// </summary>
		public int OperationId => _id;

		/// <summary>
		/// Gets a state being dismissed.
		/// </summary>
		public IAppState State => _state;

		/// <summary>
		/// Gets a controller being dismissed.
		/// </summary>
		public IViewController Controller => _controller;

		/// <summary>
		/// Initializes a new instance of the <see cref="DismissCompletedEventArgs"/> class.
		/// </summary>
		public DismissCompletedEventArgs(IAppState state, IViewController controller, int opId, object userState)
			: base(null, false, userState)
		{
			_id = opId;
			_state = state;
			_controller = controller;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DismissCompletedEventArgs"/> class.
		/// </summary>
		public DismissCompletedEventArgs(IAppState state, IViewController controller, int opId, object userState, Exception e, bool canceled)
			: base(e, canceled, userState)
		{
			_id = opId;
			_state = state;
			_controller = controller;
		}

		#endregion
	}
}
