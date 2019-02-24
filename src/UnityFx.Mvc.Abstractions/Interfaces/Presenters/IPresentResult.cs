// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Result of a present operation.
	/// </summary>
	/// <seealso cref="IViewController"/>
	/// <seealso cref="IPresenter"/>
	public interface IPresentResult
	{
		/// <summary>
		/// Raised when the <see cref="Controller"/> is dismissed.
		/// </summary>
		/// <seealso cref="Dismiss"/>
		event EventHandler Dismissed;

		/// <summary>
		/// Gets the view controller presented.
		/// </summary>
		IPresentable Controller { get; }

		/// <summary>
		/// Dismisses the controller.
		/// </summary>
		/// <seealso cref="Dismissed"/>
		void Dismiss();
	}
}
