// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Result of a present operation.
	/// </summary>
	/// <seealso cref="IPresentResult"/>
	public interface IPresentResult<TController, TResult> : IPresentResult<TController> where TController : IViewController<TResult>
	{
		/// <summary>
		/// Gets the controller result.
		/// </summary>
		TResult Result { get; }
	}
}
