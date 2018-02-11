// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A store operation.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public interface IAppStateOperationInfo
	{
		/// <summary>
		/// Gets identifier of the corresponding operation.
		/// </summary>
		/// <value>An unique operation identifier.</value>
		int OperationId { get; }

		/// <summary>
		/// Gets the operation type identifier.
		/// </summary>
		/// <value>Type of the operation.</value>
		AppStateOperationType OperationType { get; }

		/// <summary>
		/// Gets user-defined data assosiated with the operation (if any).
		/// </summary>
		/// <value>User-defined data.</value>
		object UserState { get; }
	}
}
