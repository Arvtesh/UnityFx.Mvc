// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Defines event handlers for a presentable obejct.
	/// </summary>
	/// <seealso cref="IPresentable"/>
	public interface IPresentableEvents
	{
		/// <summary>
		/// Called right after the object is ready to be used (before first <see cref="OnActivate"/>).
		/// The method is called exactly once per object lifetime.
		/// </summary>
		/// <seealso cref="OnDismiss"/>
		/// <seealso cref="OnActivate"/>
		void OnPresent();

		/// <summary>
		/// Called right before the object becomes active. The method may be called multiple times during the object lifetime.
		/// </summary>
		/// <seealso cref="OnDeactivate"/>
		/// <seealso cref="OnPresent"/>
		void OnActivate();

		/// <summary>
		/// Called when the object is about to become inactive. The method may be called multiple times during the object lifetime.
		/// </summary>
		/// <seealso cref="OnActivate"/>
		/// <seealso cref="OnDismiss"/>
		void OnDeactivate();

		/// <summary>
		/// Called when the object is about to be dismissed (after <see cref="OnDeactivate"/>). The method is called exactly once during the object lifetime.
		/// </summary>
		/// <seealso cref="OnPresent"/>
		/// <seealso cref="OnDeactivate"/>
		void OnDismiss();
	}
}
