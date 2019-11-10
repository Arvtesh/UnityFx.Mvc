// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Extensions of <see cref="IPresentResult"/>.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class IPresentResultExtensions
	{
		public struct PresentResultAwaitable
		{
			private readonly IPresentResult _presentResult;
			private readonly bool _awaitPresent;

			internal PresentResultAwaitable(IPresentResult presentResult, bool awaitPresent)
			{
				_presentResult = presentResult;
				_awaitPresent = awaitPresent;
			}

			public TaskAwaiter GetAwaiter()
			{
				if (_awaitPresent)
				{
					return _presentResult.PresentTask.GetAwaiter();
				}
				else
				{
					return _presentResult.DismissTask.GetAwaiter();
				}
			}
		}

		/// <summary>
		/// Gets an awaiter used to await this <see cref="IPresentResult"/>.
		/// </summary>
		public static PresentResultAwaitable ConfigureAwait(this IPresentResult presentResult, bool awaitPresent)
		{
			return new PresentResultAwaitable(presentResult, awaitPresent);
		}

		/// <summary>
		/// Gets an awaiter used to await this <see cref="IPresentResult"/>.
		/// </summary>
		public static TaskAwaiter GetAwaiter(this IPresentResult presentResult)
		{
			return presentResult.DismissTask.GetAwaiter();
		}

		/// <summary>
		/// Gets an awaiter used to await this <see cref="IPresentResult"/>.
		/// </summary>
		public static TaskAwaiter<TResult> GetAwaiter<TController, TResult>(this IPresentResult<TController, TResult> presentResult) where TController : IViewController
		{
			return presentResult.DismissTask.GetAwaiter();
		}
	}
}
