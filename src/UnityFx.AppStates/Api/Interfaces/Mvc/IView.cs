// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic view.
	/// </summary>
	/// <seealso cref="IViewController"/>
	public interface IView : IObjectId, IDisposable
	{
		/// <summary>
		/// Gets or sets a value indicating whether the view is visible.
		/// </summary>
		bool Visible { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the view is enabled (i.e. accepts user input).
		/// </summary>
		bool Enabled { get; set; }
	}
}
