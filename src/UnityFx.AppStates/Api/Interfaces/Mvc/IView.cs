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
	public interface IView : IObjectId, IComponent
	{
		/// <summary>
		/// Raised when the <see cref="Visible"/> property value changes.
		/// </summary>
		event EventHandler VisibleChanged;

		/// <summary>
		/// Raised when the <see cref="Enabled"/> property value changes.
		/// </summary>
		event EventHandler EnabledChanged;

		/// <summary>
		/// Gets or sets a value indicating whether the view is visible.
		/// </summary>
		bool Visible { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the view can respond to user interaction.
		/// </summary>
		bool Enabled { get; set; }
	}
}
