// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A factory for <see cref="IAppStateView"/> instances.
	/// </summary>
	/// <seealso cref="IAppStateView"/>
	public interface IAppStateViewManager
	{
		/// <summary>
		/// Creates an empty view with the specified <paramref name="id"/> on top of the <paramref name="insertAfter"/> one.
		/// If <paramref name="insertAfter"/> is <see langword="null"/> the view is created below all others.
		/// </summary>
		IAppStateView CreateView(string id, IAppStateView insertAfter);

		/// <summary>
		/// Creates a modal activity indicator.
		/// </summary>
		/// <returns></returns>
		IDisposable CreateWaitBox(string text);
	}
}
