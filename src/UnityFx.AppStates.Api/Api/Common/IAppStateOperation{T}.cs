// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A yieldable asynchronous operation with status information.
	/// </summary>
	public interface IAppStateOperation<T> : IAppStateOperation
	{
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
	}
}
