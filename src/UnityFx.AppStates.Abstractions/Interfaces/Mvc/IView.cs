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
	public interface IView : IComponent, INotifyCommand
	{
		/// <summary>
		/// Gets the view name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets or sets the object that contains data about the view.
		/// </summary>
		object Tag { get; set; }

		/// <summary>
		/// Gets or sets the view options.
		/// </summary>
		ViewOptions Options { get; set; }

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
