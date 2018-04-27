// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Event arguments for <see cref="IAppStateService.PopStateInitiated"/>.
	/// </summary>
	public class PopStateInitiatedEventArgs : EventArgs, IAppStateOperationInfo
	{
		#region data

		private readonly int _id;
		private readonly AppStateOperationType _type;
		private readonly object _userState;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="PopStateInitiatedEventArgs"/> class.
		/// </summary>
		public PopStateInitiatedEventArgs(IAppStateOperationInfo op)
		{
			_id = op.OperationId;
			_type = op.OperationType;
			_userState = op.UserState;
		}

		#endregion

		#region IAppStateOperationInfo

		/// <inheritdoc/>
		public int OperationId => _id;

		/// <inheritdoc/>
		public AppStateOperationType OperationType => _type;

		/// <inheritdoc/>
		public object UserState => _userState;

		#endregion
	}
}
