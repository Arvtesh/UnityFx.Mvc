// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Result of a present operation.
	/// </summary>
	/// <seealso cref="IPresentResult"/>
	public interface IPresentResult<TController> : IPresentResult where TController : IPresentable
	{
		/// <summary>
		/// Gets the view controller presented.
		/// </summary>
		new TController Controller { get; }

#if !NET35

		/// <summary>
		/// Gets an awaiter used to await this <see cref="IPresentResult{TController}"/>.
		/// </summary>
		/// <returns>An awaiter instance.</returns>
		new CompilerServices.IPresentAwaiter<TController> GetAwaiter();

#endif
	}
}
