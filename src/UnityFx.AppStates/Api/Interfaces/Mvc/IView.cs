// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic view.
	/// </summary>
	/// <seealso cref="IViewController"/>
	public interface IView : IComponent
	{
		/// <summary>
		/// Gets or sets the view name.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Gets or sets an arbitrary object value that can be used to store custom information about the view.
		/// </summary>
		object Tag { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the view is visible.
		/// </summary>
		/// <seealso cref="Enabled"/>
		bool Visible { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the view can respond to user interaction.
		/// </summary>
		/// <seealso cref="Visible"/>
		bool Enabled { get; set; }
	}
}
