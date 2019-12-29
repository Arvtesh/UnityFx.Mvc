// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A generic view.
	/// </summary>
	/// <remarks>
	/// In the Model-View-Controller (MVC) pattern, the view handles the app's data presentation and user interaction.
	/// A view is a <see cref="GameObject"/>. Views are created via <see cref="IViewFactory"/> instance.
	/// </remarks>
	/// <seealso cref="IViewController"/>
	/// <seealso cref="IViewFactory"/>
	/// <seealso href="https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93controller"/>
	public interface IView : IComponent, INotifyCommand
	{
		/// <summary>
		/// Gets the <see cref="Transform"/> this view is attached to.
		/// </summary>
		Transform Transform { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the view is enabled.
		/// </summary>
		bool Enabled { get; set; }
	}
}
