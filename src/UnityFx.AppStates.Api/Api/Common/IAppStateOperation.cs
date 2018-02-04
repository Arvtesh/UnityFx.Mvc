// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A yieldable asynchronous operation with status information.
	/// </summary>
	public interface IAppStateOperation : IAsyncOperation
	{
		/// <summary>
		/// Returns an operation identifier. Read only.
		/// </summary>
		int Id { get; }

		/// <summary>
		/// Returns one or more exceptions that caused the operation to end prematurely. If the operation completed successfully
		/// or has not yet thrown any exceptions, this will return <see langword="null"/>. Read only.
		/// </summary>
		IEnumerable<Exception> Exceptions { get; }
	}
}
