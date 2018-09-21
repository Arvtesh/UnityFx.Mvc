// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Defines context data for a dismiss operation.
	/// </summary>
	/// <seealso cref="IPresentable"/>
	/// <seealso cref="IViewController"/>
	/// <seealso cref="IPresentContext"/>
	public interface IDismissContext
	{
		/// <summary>
		/// Gets the next state (a state that will be activated after the dismiss operation).
		/// </summary>
		IAppState NextState { get; }
	}
}
