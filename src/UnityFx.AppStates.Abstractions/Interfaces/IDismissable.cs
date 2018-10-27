// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A dismissable object (i.e. object that supports asynchronous disposal).
	/// </summary>
	/// <seealso cref="IViewController"/>
	/// <seealso cref="IAppState"/>
	public interface IDismissable
	{
		/// <summary>
		/// Dismisses the object. Act like asynchronous <c>Dispose()</c>.
		/// </summary>
		/// <remarks>
		/// The method can be called more than once during the object lifetime. Implementations are
		/// expected to handle such cases. There is no requirement to return the same operation instance
		/// on each call.
		/// </remarks>
		/// <returns>Returns an object that can be used to track the operation state.</returns>
		IAsyncOperation DismissAsync();
	}
}
