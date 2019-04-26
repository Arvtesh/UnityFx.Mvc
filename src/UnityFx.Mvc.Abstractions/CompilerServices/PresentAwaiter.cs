// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
#if !NET35
using System.Runtime.CompilerServices;
#endif

namespace UnityFx.Mvc.CompilerServices
{
#if !NET35

	/// <summary>
	/// Provides an awaitable object that allows for configured awaits on <see cref="IPresentResult"/>.
	/// This type is intended for compiler use only.
	/// </summary>
	/// <seealso cref="IPresentResult"/>
	public struct PresentAwaiter : INotifyCompletion
	{
		private readonly IPresentResult _presentResult;

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentAwaiter"/> struct.
		/// </summary>
		public PresentAwaiter(IPresentResult presentResult)
		{
			_presentResult = presentResult;
		}

		/// <summary>
		/// Gets a value indicating whether the asynchronous task has completed.
		/// </summary>
		public bool IsCompleted => _presentResult.IsPresented;

		/// <summary>
		/// Ends the wait for the completion of the asynchronous task.
		/// </summary>
		public IPresentable GetResult() => _presentResult.Controller;

		/// <inheritdoc/>
		public void OnCompleted(Action continuation)
		{
			_presentResult.Presented += (s, e) => continuation();
		}
	}

#endif
}
