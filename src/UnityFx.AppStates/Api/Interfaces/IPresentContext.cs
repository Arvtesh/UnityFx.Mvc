// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Defines context data for a present operation.
	/// </summary>
	/// <seealso cref="IPresentable"/>
	/// <seealso cref="IViewController"/>
	/// <seealso cref="IDismissContext"/>
	public interface IPresentContext
	{
		/// <summary>
		/// Gets a state that was active before the present operation.
		/// </summary>
		IAppState PrevState { get; }
	}
}
