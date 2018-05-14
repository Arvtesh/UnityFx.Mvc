// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A dismissable object.
	/// </summary>
	public interface IDismissable : IDisposable
	{
		/// <summary>
		/// Dismisses this instance. When dismiss is complete <see cref="IDisposable.Dispose"/> is invoked.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown if either the object is disposed.</exception>
		IAsyncOperation DismissAsync();
	}
}
