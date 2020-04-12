// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Marks a class as a view controller.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ViewControllerAttribute : Attribute
	{
		/// <summary>
		/// Gets or sets default present options value.
		/// </summary>
		public PresentOptions PresentOptions { get; set; }

		/// <summary>
		/// Gets or sets controller tag.
		/// </summary>
		public int Tag { get; set; }

		/// <summary>
		/// Gets or sets layer index.
		/// </summary>
		public int Layer { get; set; }
	}
}
