// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Custom view binding.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ViewControllerBindingAttribute : Attribute
	{
		/// <summary>
		/// Gets or sets controller type this view is designed for.
		/// </summary>
		public Type ControllerType { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewControllerBindingAttribute"/> class.
		/// </summary>
		public ViewControllerBindingAttribute(Type controllerType)
		{
			ControllerType = controllerType;
		}
	}
}
