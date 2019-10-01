// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A dismissable entity
	/// </summary>
	public interface IDismissable : IDisposable
	{
		/// <summary>
		/// Raised when the entity is dismissed.
		/// </summary>
		/// <seealso cref="Dismiss"/>
		event EventHandler Dismissed;

		/// <summary>
		/// Gets a value indicating whether the entity is dismissed.
		/// </summary>
		/// <seealso cref="Dismiss"/>
		/// <seealso cref="Dismissed"/>
		bool IsDismissed { get; }

		/// <summary>
		/// Dismisses the instance.
		/// </summary>
		/// <seealso cref="IsDismissed"/>
		/// <seealso cref="Dismissed"/>
		void Dismiss();
	}
}
