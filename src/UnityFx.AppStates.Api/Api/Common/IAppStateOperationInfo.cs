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
		/// Returns identifier of the corresponding operation (matches <see cref="IAppStateOperation.Id"/>). Read only.
		/// </summary>
		/// <value>An unique operation identifier.</value>
		int OperationId { get; }

		/// <summary>
		/// Returns the operation type identifier. Read only.
		/// </summary>
		/// <value>Type of the operation.</value>
		AppStateOperationType OperationType { get; }

		/// <summary>
		/// Returns user-defined data assosiated with the operation (if any). Read only.
		/// </summary>
		/// <value>User-defined data.</value>
		object UserState { get; }
	}
}
