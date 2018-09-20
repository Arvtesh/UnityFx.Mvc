// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Defines middleware that can be added to the application's present pipeline.
	/// </summary>
	/// <seealso cref="IPresentPipelineBuilder"/>
	public interface IPresentMiddleware
	{
		/// <summary>
		/// Defines a method that is called when presenting a controller instance.
		/// </summary>
		IAsyncOperation InvokeAsync(IPresentContext context);
	}
}
