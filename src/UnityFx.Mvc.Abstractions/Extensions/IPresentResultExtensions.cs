// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Extensions of <see cref="IPresentResult"/>.
	/// </summary>
	/// <seealso cref="IPresentResult"/>
	public static class IPresentResultExtensions
	{
#if !NET35

		/// <summary>
		/// Gets an awaiter used to await this <see cref="IPresentResult"/>.
		/// </summary>
		public static CompilerServices.PresentAwaiter GetAwaiter(this IPresentResult presentResult)
		{
			return new CompilerServices.PresentAwaiter(presentResult);
		}

		/// <summary>
		/// Gets an awaiter used to await this <see cref="IPresentResult"/>.
		/// </summary>
		public static CompilerServices.PresentAwaiter<TController> GetAwaiter<TController>(this IPresentResult<TController> presentResult) where TController : IPresentable
		{
			return new CompilerServices.PresentAwaiter<TController>(presentResult);
		}

#endif
	}
}
