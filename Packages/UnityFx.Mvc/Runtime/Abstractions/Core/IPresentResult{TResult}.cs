// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Result of a present operation with a result.
	/// </summary>
	/// <typeparam name="TResult">Type of the result value.</typeparam>
	/// <seealso cref="IPresentResult"/>
	public interface IPresentResult<TResult> : IPresentResult, IViewControllerResultAccess<TResult>
	{
		/// <summary>
		/// Gets a <see cref="Task{TResult}"/> instance that can be used to await the operation completion.
		/// </summary>
		new Task<TResult> Task { get; }
	}
}
