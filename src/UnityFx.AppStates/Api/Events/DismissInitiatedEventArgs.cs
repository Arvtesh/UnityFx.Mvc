// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Event arguments for <see cref="IAppStateService.DismissInitiated"/>.
	/// </summary>
	public class DismissInitiatedEventArgs : EventArgs, IAppOperationInfo
	{
		#region data

		private readonly int _id;
		private readonly object _userState;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="DismissInitiatedEventArgs"/> class.
		/// </summary>
		public DismissInitiatedEventArgs(IAppOperationInfo op)
		{
			_id = op.OperationId;
			_userState = op.UserState;
		}

		#endregion

		#region IAppStateOperationInfo

		/// <inheritdoc/>
		public int OperationId => _id;

		/// <inheritdoc/>
		public object UserState => _userState;

		#endregion
	}
}
