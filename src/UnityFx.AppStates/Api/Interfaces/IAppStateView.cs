// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic application view.
	/// </summary>
	/// <seealso cref="IAppStateViewManager"/>
	public interface IAppStateView : IDisposable
	{
		/// <summary>
		/// Gets the view identifier (matches the state identifier).
		/// </summary>
		string Id { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the view is visible.
		/// </summary>
		bool Visible { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether input processing is enabled for the view. Note that disabled views do not process input.
		/// </summary>
		bool Interactable { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the view should cover all screen (views under it are not visible).
		/// </summary>
		bool Exclusive { get; set; }
	}
}
