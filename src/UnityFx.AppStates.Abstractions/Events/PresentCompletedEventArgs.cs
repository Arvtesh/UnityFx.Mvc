// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Event arguments for <see cref="IAppStateService.PresentCompleted"/>.
	/// </summary>
	public class PresentCompletedEventArgs : AsyncCompletedEventArgs
	{
		#region data

		private readonly int _id;
		private readonly IAppState _state;
		private readonly IViewController _controller;

		#endregion

		#region interface

		/// <summary>
		/// Gets identifier of the present operation.
		/// </summary>
		public int OperationId => _id;

		/// <summary>
		/// Gets the state presented.
		/// </summary>
		public IAppState State => _state;

		/// <summary>
		/// Gets the controller presented.
		/// </summary>
		public IViewController Controller => _controller;

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentCompletedEventArgs"/> class.
		/// </summary>
		public PresentCompletedEventArgs(IAppState state, IViewController controller, int opId, object userState)
			: base(null, false, userState)
		{
			_id = opId;
			_state = state;
			_controller = controller;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentCompletedEventArgs"/> class.
		/// </summary>
		public PresentCompletedEventArgs(IAppState state, IViewController controller, int opId, object userState, Exception e, bool canceled)
			: base(e, canceled, userState)
		{
			_id = opId;
			_state = state;
			_controller = controller;
		}

		#endregion
	}
}
