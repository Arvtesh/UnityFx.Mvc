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
		/// Dismisses this instance. When dismiss is complete <see cref="IDisposable.Dispose"/> is invoked.
		/// </summary>
		/// <remarks>
		/// Just like <see cref="IDisposable.Dispose"/> this method can be safely called multiple times even
		/// if the object has been disposed.
		/// </remarks>
		IAsyncOperation DismissAsync();
	}
}
