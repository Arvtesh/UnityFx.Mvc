// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A presentable event handlers.
	/// </summary>
	/// <seealso cref="IPresentable"/>
	public interface IPresentableEvents
	{
		/// <summary>
		/// Called when the controller view is loaded (before transition animation).
		/// </summary>
		void OnViewLoaded();

		/// <summary>
		/// Called right after the controller transition animation finishes.
		/// </summary>
		void OnPresent();

		/// <summary>
		/// Called right before the controller becomes active.
		/// </summary>
		void OnActivate();

		/// <summary>
		/// Called when the controller is about to become inactive.
		/// </summary>
		void OnDeactivate();

		/// <summary>
		/// Called when the controller is about to be dismissed (before transition animation).
		/// </summary>
		void OnDismiss();
	}
}
