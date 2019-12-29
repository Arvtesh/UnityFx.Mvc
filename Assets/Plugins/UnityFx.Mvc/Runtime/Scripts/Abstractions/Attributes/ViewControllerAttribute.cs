// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Marks a class as a view controller.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ViewControllerAttribute : Attribute
	{
		/// <summary>
		/// Gets or sets name of the view prefab.
		/// </summary>
		public string ViewPrefabName { get; set; }

		/// <summary>
		/// Gets or sets index of the view layer.
		/// </summary>
		public int ViewLayer { get; set; }

		/// <summary>
		/// Gets or sets present options.
		/// </summary>
		public PresentOptions PresentOptions { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewControllerAttribute"/> class.
		/// </summary>
		public ViewControllerAttribute()
		{
		}
	}
}
