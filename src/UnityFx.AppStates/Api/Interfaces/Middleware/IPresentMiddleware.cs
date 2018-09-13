// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Defines middleware that can be added to the application's controller pipeline.
	/// </summary>
	/// <seealso cref="IViewController"/>
	public interface IPresentMiddleware
	{
		/// <summary>
		/// Dismisses this instance.
		/// </summary>
		IAsyncOperation PresentAsync(IViewController controller, IPresentContext context);

		/// <summary>
		/// Dismisses this instance.
		/// </summary>
		IAsyncOperation DismissAsync(IViewController controller);
	}
}
