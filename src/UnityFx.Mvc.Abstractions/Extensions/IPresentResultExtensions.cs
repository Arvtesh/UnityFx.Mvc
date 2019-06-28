// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Extensions of <see cref="IPresentResult"/>.
	/// </summary>
	/// <seealso cref="IPresentResult"/>
	public static class IPresentResultExtensions
	{
		/// <summary>
		/// Gets an awaiter used to await this <see cref="IPresentResult"/>.
		/// </summary>
		public static TaskAwaiter<IViewController> GetAwaiter(this IPresentResult presentResult)
		{
			return presentResult.PresentTask.GetAwaiter();
		}

		/// <summary>
		/// Gets an awaiter used to await this <see cref="IPresentResult{TController}"/>.
		/// </summary>
		public static TaskAwaiter<TController> GetAwaiter<TController>(this IPresentResult<TController> presentResult) where TController : IViewController
		{
			return presentResult.PresentTask.GetAwaiter();
		}
	}
}
