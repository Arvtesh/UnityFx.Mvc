// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Marks a route for a controller.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class RouteAttribute : Attribute
	{
		/// <summary>
		/// Gets the route template. May be <see langword="null"/>.
		/// </summary>
		public string Template { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="RouteAttribute"/> class.
		/// </summary>
		public RouteAttribute(string template)
		{
			Template = template;
		}
	}
}
