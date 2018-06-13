// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Event arguments for <see cref="IAppStateService.PresentInitiated"/>.
	/// </summary>
	public class PresentInitiatedEventArgs : EventArgs
	{
		#region data

		private readonly int _id;
		private readonly object _userState;
		private readonly PresentOptions _options;

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
		/// Gets present options.
		/// </summary>
		public PresentOptions Options => _options;

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentInitiatedEventArgs"/> class.
		/// </summary>
		public PresentInitiatedEventArgs(PresentOptions options, int opId, object userState)
		{
			_id = opId;
			_userState = userState;
			_options = options;
		}

		#endregion
	}
}
