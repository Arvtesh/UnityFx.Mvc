// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Extensions of <see cref="IPresentResult"/> interface.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class PresentResultExtensions
	{
		/// <summary>
		/// Returns result value of the view controller if it's available.
		/// </summary>
		/// <typeparam name="TResult">Type of the result value.</typeparam>
		public static TResult GetResult<TResult>(this IPresentResult presentResult)
		{
			return ((IViewControllerResultAccess<TResult>)presentResult).Result;
		}

		/// <summary>
		/// Returns result value of the view controller when it's available.
		/// </summary>
		/// <typeparam name="TResult">Type of the result value.</typeparam>
		public static Task<TResult> GetResultAsync<TResult>(this IPresentResult presentResult)
		{
			return ((IViewControllerResultAccess<TResult>)presentResult).Task;
		}
	}
}
