// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A presentable event handlers.
	/// </summary>
	/// <seealso cref="IViewController"/>
	public interface IViewControllerEvents
	{
		/// <summary>
		/// Called when the controller view is loaded (before transition animation).
		/// </summary>
		/// <seealso cref="OnPresent"/>
		void OnViewLoaded();

		/// <summary>
		/// Called right after the controller transition animation finishes.
		/// </summary>
		/// <seealso cref="OnDismiss"/>
		/// <seealso cref="OnViewLoaded"/>
		void OnPresent();

		/// <summary>
		/// Called right before the controller becomes active.
		/// </summary>
		/// <seealso cref="OnDeactivate"/>
		void OnActivate();

		/// <summary>
		/// Called when the controller is about to become inactive.
		/// </summary>
		/// <seealso cref="OnActivate"/>
		void OnDeactivate();

		/// <summary>
		/// Called when the controller is about to be dismissed (before transition animation).
		/// </summary>
		/// <seealso cref="OnPresent"/>
		void OnDismiss();
	}
}
