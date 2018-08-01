// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A presentable event handlers.
	/// </summary>
	public interface IPresentableEvents
	{
		/// <summary>
		/// Called when the controller view is loaded (before transition animation). Default implementation does nothing.
		/// </summary>
		void OnViewLoaded();

		/// <summary>
		/// Called right after the controller transition animation finishes. Default implementation does nothing.
		/// </summary>
		void OnPresent();

		/// <summary>
		/// Called right before the controller becomes active. Default implementation does nothing.
		/// </summary>
		void OnActivate();

		/// <summary>
		/// Called when the controller is about to become inactive. Default implementation does nothing.
		/// </summary>
		void OnDeactivate();

		/// <summary>
		/// Called when the controller is about to be dismissed (before transition animation). Default implementation does nothing.
		/// </summary>
		void OnDismiss();
	}
}
