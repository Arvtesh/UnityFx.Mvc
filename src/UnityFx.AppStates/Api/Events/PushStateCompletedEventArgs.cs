﻿// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Event arguments for <see cref="IAppStateService.PushStateCompleted"/>.
	/// </summary>
	public class PushStateCompletedEventArgs : AsyncCompletedEventArgs, IAppStateOperationInfo
	{
		#region data

		private readonly int _id;
		private readonly AppStateOperationType _type;
		private readonly AppState _state;

		#endregion

		#region interface

		/// <summary>
		/// Gets a target state.
		/// </summary>
		public AppState State => _state;

		/// <summary>
		/// Initializes a new instance of the <see cref="PushStateCompletedEventArgs"/> class.
		/// </summary>
		public PushStateCompletedEventArgs(IAppStateOperationInfo op, AppState state)
			: base(null, false, op.UserState)
		{
			_id = op.OperationId;
			_type = op.OperationType;
			_state = state;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PushStateCompletedEventArgs"/> class.
		/// </summary>
		public PushStateCompletedEventArgs(IAppStateOperationInfo op, AppState state, Exception e, bool canceled)
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
