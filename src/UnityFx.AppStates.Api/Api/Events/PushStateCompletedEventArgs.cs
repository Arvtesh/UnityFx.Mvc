// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

namespace UnityFx.AppStates
{
	/// <summary>
	/// tt
	/// </summary>
	public class PushStateCompletedEventArgs : AsyncCompletedEventArgs, IAppStateOperationInfo
	{
		#region data

		private readonly int _id;
		private readonly AppStateOperationType _type;
		private readonly IAppState _state;

		#endregion

		#region interface

		/// <summary>
		/// tt
		/// </summary>
		public IAppState State => _state;

		/// <summary>
		/// Initializes a new instance of the <see cref="PushStateCompletedEventArgs"/> class.
		/// </summary>
		public PushStateCompletedEventArgs(IAppStateOperationInfo op, IAppState state)
			: base(null, false, op.UserState)
		{
			_id = op.OperationId;
			_type = op.OperationType;
			_state = state;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PushStateCompletedEventArgs"/> class.
		/// </summary>
		public PushStateCompletedEventArgs(IAppStateOperationInfo op, IAppState state, Exception e, bool canceled)
			: base(e, canceled, op.UserState)
		{
			_id = op.OperationId;
			_type = op.OperationType;
			_state = state;
		}

		#endregion

		#region IAppStateOperationInfo

		/// <inheritdoc/>
		public int OperationId => _id;

		/// <inheritdoc/>
		public AppStateOperationType OperationType => _type;

		#endregion
	}
}
