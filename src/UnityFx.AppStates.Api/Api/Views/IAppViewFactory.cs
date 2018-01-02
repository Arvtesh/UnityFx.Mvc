// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A factory for <see cref="IAppView"/> instances.
	/// </summary>
	/// <seealso cref="IAppView"/>
	public interface IAppViewFactory
	{
		/// <summary>
		/// Creates an empty view with the specified <paramref name="name"/> on top of the <paramref name="insertAfter"/> one.
		/// If <paramref name="insertAfter"/> is <see langword="null"/> the view is created below all others.
		/// </summary>
		IAppView CreateView(string name, IAppView insertAfter);
	}
}
