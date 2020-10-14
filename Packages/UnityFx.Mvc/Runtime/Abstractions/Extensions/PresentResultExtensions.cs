// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using System.Threading;
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
		/// <typeparam name="TResult">Type of the controller result value.</typeparam>
		public static TResult GetResult<TResult>(this IPresentResult presentResult)
		{
			return ((IViewControllerResultAccess<TResult>)presentResult).Result;
		}

		/// <summary>
		/// Returns result value of the view controller when it's available.
		/// </summary>
		/// <typeparam name="TResult">Type of the controller result value.</typeparam>
		public static Task<TResult> GetResultAsync<TResult>(this IPresentResult presentResult)
		{
			return ((IViewControllerResultAccess<TResult>)presentResult).Task;
		}

		/// <summary>
		/// Dismisses the controller as soon as the <paramref name="cancellationToken"/> is cancelled.
		/// </summary>
		public static IPresentResult WithCancellation(this IPresentResult presentResult, CancellationToken cancellationToken)
		{
			if (cancellationToken.CanBeCanceled)
			{
				cancellationToken.Register(
					() =>
					{
						presentResult.Dismiss(cancellationToken);
					},
					true);
			}

			return presentResult;
		}

		/// <summary>
		/// Dismisses the controller as soon as the <paramref name="cancellationToken"/> is cancelled.
		/// </summary>
		/// <typeparam name="TResult">Type of the controller result value.</typeparam>
		public static IPresentResult<TResult> WithCancellation<TResult>(this IPresentResult<TResult> presentResult, CancellationToken cancellationToken)
		{
			if (cancellationToken.CanBeCanceled)
			{
				cancellationToken.Register(
					() =>
					{
						presentResult.Dismiss(cancellationToken);
					},
					true);
			}

			return presentResult;
		}
	}
}
