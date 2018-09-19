// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Defines a class that provides the mechanisms to configure an application's present pipeline.
	/// </summary>
	/// <seealso cref="IPresentMiddleware"/>
	public interface IPresentPipelineBuilder
	{
		/// <summary>
		/// Gets application service provider.
		/// </summary>
		IServiceProvider ServiceProvider { get; }

		/// <summary>
		/// Gets a key/value collection that can be used to share data between middleware.
		/// </summary>
		IDictionary<string, object> Properties { get; }

		/// <summary>
		/// Adds a middleware instance to the application's present pipeline.
		/// </summary>
		void Use(IPresentMiddleware middleware);

		/// <summary>
		/// Adds a middleware instance to the application's present pipeline.
		/// </summary>
		void UseWhen(IPresentMiddleware middleware, Predicate<IPresentContext> predicate);

		/// <summary>
		/// Builds the middleware used by this application to process present commands.
		/// </summary>
		IPresentMiddleware Build(IPresentContext context);
	}
}
