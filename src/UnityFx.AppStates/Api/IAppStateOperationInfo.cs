// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.App
{
	/// <summary>
	/// A result of a state operation.
	/// </summary>
	/// <seealso cref="IAppStateManager"/>
	public interface IAppStateOperationInfo
	{
		/// <summary>
		/// Returns the operation type. Read only.
		/// </summary>
		AppStateOperationType Type { get; }

		/// <summary>
		/// Returns operation arguments (if any). Read only.
		/// </summary>
		/// <seealso cref="Result"/>
		object Args { get; }

		/// <summary>
		/// Returns the operation result (if available). Read only.
		/// </summary>
		/// <seealso cref="Target"/>
		IAppState Result { get; }

		/// <summary>
		/// Returns the state that is the subject of the operation. Read only.
		/// </summary>
		/// <seealso cref="Result"/>
		IAppState Target { get; }

		/// <summary>
		/// Returns exception (if any). Read only.
		/// </summary>
		AggregateException Exception { get; }

		/// <summary>
		/// Returns <see langword="true"/> if the operation succeeded; <see langword="false"/> otherwise. Read only.
		/// </summary>
		/// <seealso cref="IsCanceled"/>
		/// <seealso cref="IsFaulted"/>
		bool IsSucceeded { get; }

		/// <summary>
		/// Returns <see langword="true"/> if the operation was canceled; <see langword="false"/> otherwise. Read only.
		/// </summary>
		/// <seealso cref="IsCanceled"/>
		/// <seealso cref="IsSucceeded"/>
		bool IsCanceled { get; }

		/// <summary>
		/// Returns <see langword="true"/> if the operation failed; <see langword="false"/> otherwise. Read only.
		/// </summary>
		/// <seealso cref="IsCanceled"/>
		/// <seealso cref="IsSucceeded"/>
		bool IsFaulted { get; }
	}
}