// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A yieldable asynchronous operation with status information.
	/// </summary>
	public interface IAppStateOperation
	{
		/// <summary>
		/// Returns an operation identifier. Read only.
		/// </summary>
		int Id { get; }

		/// <summary>
		/// Returns the very first of exceptions that caused the operation to end prematurely. If the operation completed successfully
		/// or has not yet thrown any exceptions, this will return <see langword="null"/>. Read only.
		/// </summary>
		/// <seealso cref="Exceptions"/>
		/// <seealso cref="IsFaulted"/>
		Exception Exception { get; }

		/// <summary>
		/// Returns one or more exceptions that caused the operation to end prematurely. If the operation completed successfully
		/// or has not yet thrown any exceptions, this will return <see langword="null"/>. Read only.
		/// </summary>
		/// <seealso cref="Exception"/>
		/// <seealso cref="IsFaulted"/>
		IEnumerable<Exception> Exceptions { get; }

		/// <summary>
		/// Returns <see langword="true"/> if the operation has completed successfully, <see langword="false"/> otherwise. Read only.
		/// </summary>
		/// <seealso cref="IsCompleted"/>
		/// <seealso cref="IsFaulted"/>
		/// <seealso cref="IsCanceled"/>
		bool IsCompletedSuccessfully { get; }

		/// <summary>
		/// Returns <see langword="true"/> if the operation has completed (either successfully or not), <see langword="false"/> otherwise. Read only.
		/// </summary>
		/// <seealso cref="IsCompletedSuccessfully"/>
		/// <seealso cref="IsFaulted"/>
		/// <seealso cref="IsCanceled"/>
		bool IsCompleted { get; }

		/// <summary>
		/// Returns <see langword="true"/> if the operation has failed for any reason, <see langword="false"/> otherwise. Read only.
		/// </summary>
		/// <seealso cref="Exception"/>
		/// <seealso cref="IsCompleted"/>
		/// <seealso cref="IsCompletedSuccessfully"/>
		/// <seealso cref="IsCanceled"/>
		bool IsFaulted { get; }

		/// <summary>
		/// Returns <see langword="true"/> if the operation has been canceled by user, <see langword="false"/> otherwise. Read only.
		/// </summary>
		/// <seealso cref="IsCompleted"/>
		/// <seealso cref="IsCompletedSuccessfully"/>
		/// <seealso cref="IsFaulted"/>
		bool IsCanceled { get; }
	}
}
