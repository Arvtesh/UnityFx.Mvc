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
		/// Creates an empty view with the specified name.
		/// </summary>
		IAppView CreateView(string name, IAppView insertAfter);
	}
}
