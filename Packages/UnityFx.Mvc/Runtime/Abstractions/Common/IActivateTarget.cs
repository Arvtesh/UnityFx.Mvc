// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A target for ACTIVATE/DEACTIVATE notifications.
	/// </summary>
	/// <seealso cref="IPresentTarget"/>
	/// <seealso cref="IViewController"/>
	public interface IActivateTarget
	{
		/// <summary>
		/// Called right before the object becomes active. The method may be called multiple times during the object lifetime.
		/// </summary>
		/// <seealso cref="Deactivate"/>
		void Activate();

		/// <summary>
		/// Called when the object is about to become inactive. The method may be called multiple times during the object lifetime.
		/// </summary>
		/// <seealso cref="Activate"/>
		void Deactivate();
	}
}
