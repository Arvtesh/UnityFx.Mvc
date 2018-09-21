// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A presentable object (i.e. object that supports asynchronous present operation).
	/// </summary>
	/// <seealso cref="IPresenter"/>
	/// <seealso cref="IPresentableEvents"/>
	public interface IPresentable
	{
		/// <summary>
		/// Performs any asynchronous actions needed to present this object. The method is invoked by the system.
		/// </summary>
		/// <param name="presentContext">Context data provided by the system.</param>
		/// <returns>Returns an object that can be used to track the operation state.</returns>
		IAsyncOperation PresentAsync(IPresentContext presentContext);

		/// <summary>
		/// Performs any asynchronous actions needed to dismiss this object. The method is invoked by the system.
		/// </summary>
		/// <param name="dismissContext">Context data provided by the system.</param>
		/// <returns>Returns an object that can be used to track the operation state.</returns>
		IAsyncOperation DismissAsync(IDismissContext dismissContext);
	}
}
