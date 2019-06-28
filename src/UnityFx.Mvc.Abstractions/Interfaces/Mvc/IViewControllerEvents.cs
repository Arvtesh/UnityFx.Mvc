// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Defines event handlers for a presentable obejct.
	/// </summary>
	/// <seealso cref="IViewController"/>
	public interface IViewControllerEvents
	{
		/// <summary>
		/// Called right before the object becomes active. The method may be called multiple times during the object lifetime.
		/// </summary>
		/// <seealso cref="OnDeactivate"/>
		void OnActivate();

		/// <summary>
		/// Called when the object is about to become inactive. The method may be called multiple times during the object lifetime.
		/// </summary>
		/// <seealso cref="OnActivate"/>
		void OnDeactivate();
	}
}
