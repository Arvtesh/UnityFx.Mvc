// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A dismissable object.
	/// </summary>
	/// <seealso cref="IPresentable"/>
	public interface IDismissable
	{
		/// <summary>
		/// Raised when the instance is dismissed.
		/// </summary>
		/// <seealso cref="Dismiss"/>
		/// <seealso cref="IsDismissed"/>
		event EventHandler Dismissed;

		/// <summary>
		/// Gets a value indicating whether the object is dismissed.
		/// </summary>
		/// <seealso cref="Dismiss"/>
		/// <seealso cref="Dismissed"/>
		bool IsDismissed { get; }

		/// <summary>
		/// Dismisses the obejct.
		/// </summary>
		/// <seealso cref="Dismissed"/>
		/// <seealso cref="IsDismissed"/>
		void Dismiss();
	}
}
