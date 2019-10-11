// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Extensions of <see cref="IPresentResult"/>.
	/// </summary>
	public static class IPresentResultExtensions
	{
		/// <summary>
		/// Gets an awaiter used to await this <see cref="IPresentResult"/>.
		/// </summary>
		public static TaskAwaiter GetAwaiter(this IPresentResult presentResult)
		{
			return presentResult.Task.GetAwaiter();
		}

		/// <summary>
		/// Gets an awaiter used to await this <see cref="IPresentResult"/>.
		/// </summary>
		public static TaskAwaiter<TResult> GetAwaiter<TController, TResult>(this IPresentResult<TController, TResult> presentResult) where TController : IViewController
		{
			return presentResult.Task.GetAwaiter();
		}
	}
}
