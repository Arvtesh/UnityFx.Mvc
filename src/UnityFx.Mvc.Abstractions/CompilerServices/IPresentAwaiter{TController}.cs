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
	/// Provides an awaitable object that allows for configured awaits on <see cref="IPresentResult{TController}"/>.
	/// This type is intended for compiler use only.
	/// </summary>
	/// <seealso cref="IPresentResult{TController}"/>
	public interface IPresentAwaiter<TController> : INotifyCompletion where TController : IPresentable
	{
		/// <summary>
		/// Gets a value indicating whether the asynchronous task has completed.
		/// </summary>
		bool IsCompleted { get; }

		/// <summary>
		/// Ends the wait for the completion of the asynchronous task.
		/// </summary>
		TController GetResult();
	}

#endif
}
