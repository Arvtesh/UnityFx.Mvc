// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Event arguments for <see cref="IAppStateManager.PushStateInitiated"/>.
	/// </summary>
	public class PushStateInitiatedEventArgs : EventArgs, IAppStateOperationInfo
	{
		#region data

		private readonly int _id;
		private readonly AppStateOperationType _type;
		private readonly object _userState;
		private readonly PushOptions _options;

		#endregion

		#region interface

		/// <summary>
		/// tt
		/// </summary>
		public PushOptions Options => _options;

		/// <summary>
		/// Initializes a new instance of the <see cref="PushStateInitiatedEventArgs"/> class.
		/// </summary>
		public PushStateInitiatedEventArgs(IAppStateOperationInfo op, PushOptions options)
		{
			_id = op.OperationId;
			_type = op.OperationType;
			_userState = op.UserState;
			_options = options;
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
