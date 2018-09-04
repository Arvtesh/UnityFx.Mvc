// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Stores resource identifier for a view instance.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class ViewResourceAttribute : Attribute
	{
		/// <summary>
		/// Gets an identifier that is used to identify view resource attached to the controller.
		/// </summary>
		public string ResourceId { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewResourceAttribute"/> class.
		/// </summary>
		public ViewResourceAttribute(string resourceId)
		{
			ResourceId = resourceId;
		}
	}
}
