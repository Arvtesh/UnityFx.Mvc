// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Event arguments for <see cref="IPresentService.DismissInitiated"/>.
	/// </summary>
	public class DismissInitiatedEventArgs : EventArgs
	{
		#region data

		private readonly int _id;
		private readonly object _userState;
		private readonly IViewController _controller;

		#endregion

		#region interface

		/// <summary>
		/// Gets identifier of the dismiss operation.
		/// </summary>
		public int OperationId => _id;

		/// <summary>
		/// Gets user-defined data assosisated with the operation.
		/// </summary>
		public object UserState => _userState;

		/// <summary>
		/// Gets a controller being dismissed.
		/// </summary>
		public IViewController Controller => _controller;

		/// <summary>
		/// Initializes a new instance of the <see cref="DismissInitiatedEventArgs"/> class.
		/// </summary>
		public DismissInitiatedEventArgs(IViewController controller, int opId, object userState)
		{
			_id = opId;
			_userState = userState;
			_controller = controller;
		}

		#endregion
	}
}
