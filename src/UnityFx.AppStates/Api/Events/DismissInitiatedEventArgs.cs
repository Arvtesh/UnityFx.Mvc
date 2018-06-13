// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Event arguments for <see cref="IAppStateService.DismissInitiated"/>.
	/// </summary>
	public class DismissInitiatedEventArgs : EventArgs
	{
		#region data

		private readonly int _id;
		private readonly object _userState;
		private readonly IDismissable _obj;

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
		/// Gets an object being dismissed.
		/// </summary>
		public IDismissable Target => _obj;

		/// <summary>
		/// Initializes a new instance of the <see cref="DismissInitiatedEventArgs"/> class.
		/// </summary>
		public DismissInitiatedEventArgs(IDismissable obj, int opId, object userState)
		{
			_id = opId;
			_userState = userState;
			_obj = obj;
		}

		#endregion
	}
}
