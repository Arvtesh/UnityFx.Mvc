// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Event arguments for <see cref="IAppStateService.PresentInitiated"/>.
	/// </summary>
	public class PresentInitiatedEventArgs : EventArgs, IAppStateOperationInfo
	{
		#region data

		private readonly int _id;
		private readonly object _userState;
		private readonly PresentOptions _options;

		#endregion

		#region interface

		/// <summary>
		/// Gets push options.
		/// </summary>
		public PresentOptions Options => _options;

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentInitiatedEventArgs"/> class.
		/// </summary>
		public PresentInitiatedEventArgs(IAppStateOperationInfo op, PresentOptions options)
		{
			_id = op.OperationId;
			_userState = op.UserState;
			_options = options;
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
