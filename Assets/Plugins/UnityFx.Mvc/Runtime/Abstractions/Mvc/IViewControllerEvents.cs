// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Defines event handlers for a <see cref="IViewController"/> implementation.
	/// </summary>
	/// <seealso cref="IViewController"/>
	public interface IViewControllerEvents
	{
		/// <summary>
		/// Called when the object is presented.
		/// </summary>
		/// <seealso cref="OnDismiss"/>
		void OnPresent();

		/// <summary>
		/// Called when the object is dismissed.
		/// </summary>
		/// <seealso cref="OnPresent"/>
		void OnDismiss();

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
