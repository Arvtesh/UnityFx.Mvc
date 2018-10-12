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
		/// <seealso cref="Visible"/>
		/// <seealso cref="EnabledChanged"/>
		event EventHandler VisibleChanged;

		/// <summary>
		/// Raised when the <see cref="Enabled"/> property value changes.
		/// </summary>
		/// <seealso cref="Enabled"/>
		/// <seealso cref="VisibleChanged"/>
		event EventHandler EnabledChanged;

		/// <summary>
		/// Gets or sets a value indicating whether the view is visible.
		/// </summary>
		/// <seealso cref="VisibleChanged"/>
		/// <seealso cref="Enabled"/>
		bool Visible { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the view can respond to user interaction.
		/// </summary>
		/// <seealso cref="EnabledChanged"/>
		/// <seealso cref="Visible"/>
		bool Enabled { get; set; }

		/// <summary>
		/// Gets or sets an arbitrary object value that can be used to store custom information about this object.
		/// </summary>
		object Tag { get; set; }
	}
}
