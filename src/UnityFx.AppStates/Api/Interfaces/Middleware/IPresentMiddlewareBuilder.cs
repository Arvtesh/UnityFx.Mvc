// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Defines a middleware builder.
	/// </summary>
	/// <seealso cref="IPresentMiddleware"/>
	public interface IPresentMiddlewareBuilder
	{
		/// <summary>
		/// tt.
		/// </summary>
		void Use(IPresentMiddleware middleware);

		/// <summary>
		/// tt.
		/// </summary>
		void UseWhen(IPresentMiddleware middleware, Predicate<IPresentContext> predicate);

		/// <summary>
		/// tt.
		/// </summary>
		IPresentMiddleware Build(IPresentContext context);
	}
}
