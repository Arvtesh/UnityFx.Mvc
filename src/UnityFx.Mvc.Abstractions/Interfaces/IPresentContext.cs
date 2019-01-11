// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Defines context data for a present operation.
	/// </summary>
	/// <seealso cref="IAsyncPresentable"/>
	/// <seealso cref="IViewController"/>
	/// <seealso cref="IDismissContext"/>
	public interface IPresentContext
	{
		/// <summary>
		/// Gets a controller that was active before the present operation.
		/// </summary>
		IViewController PrevController { get; }
	}
}
