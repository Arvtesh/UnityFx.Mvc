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
	public interface IPresentResult : IDismissable
	{
		/// <summary>
		/// Raised when the <see cref="Controller"/> is presented.
		/// </summary>
		/// <seealso cref="IsPresented"/>
		/// <seealso cref="Controller"/>
		event EventHandler Presented;

		/// <summary>
		/// Gets a value indicating whether the <see cref="Controller"/> is presented.
		/// </summary>
		/// <seealso cref="Presented"/>
		/// <seealso cref="Controller"/>
		bool IsPresented { get; }

		/// <summary>
		/// Gets the view controller presented.
		/// </summary>
		IPresentable Controller { get; }
	}
}
