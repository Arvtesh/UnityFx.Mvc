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
		/// Returns the operation type. Read only.
		/// </summary>
		AppStateOperationType Type { get; }

		/// <summary>
		/// Returns operation arguments (if any). Read only.
		/// </summary>
		object Args { get; }

		/// <summary>
		/// Returns the result value of this operation (if any). Read only.
		/// </summary>
		/// <remarks>
		/// Once the result of an operation is available, it is stored and is returned immediately on subsequent calls to the <see cref="Result"/> property.
		/// Note that, if an exception occurred during the operation, or if the operation has been cancelled, the <see cref="Result"/> property does not return a value.
		/// Instead, attempting to access the property value throws an <see cref="InvalidOperationException"/> exception.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if the property is accessed before operation is completed or if the operation has failed.</exception>
		IAppState Result { get; }

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
		IReadOnlyCollection<Exception> Exceptions { get; }

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
