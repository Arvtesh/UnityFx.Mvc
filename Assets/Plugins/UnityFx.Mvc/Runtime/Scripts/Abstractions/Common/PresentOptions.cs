// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Enumerates state controller push options.
	/// </summary>
	/// <seealso cref="IPresenter"/>
	/// <seealso cref="IViewController"/>
	[Flags]
	public enum PresentOptions
	{
		/// <summary>
		/// Default options. The new state is pushed onto the stack.
		/// </summary>
		None = 0,

		/// <summary>
		/// Marks the controller as exclusive. Exclusive controllers cover all other controllers below. Cannot be combined with <see cref="Popup"/>.
		/// </summary>
		Exclusive = 1,

		/// <summary>
		/// Marks the controller as popup. Cannot be combined with <see cref="Exclusive"/>.
		/// </summary>
		Popup = 2,

		/// <summary>
		/// Marks the controller as modal. Modal controllers do not forward unprocessed commands to controllers below them is the stack.
		/// </summary>
		Modal = 4,

		/// <summary>
		/// If set the caller presenter is dismissed.
		/// </summary>
		DismissCurrent = 0x1000,

		/// <summary>
		/// If set the caller presenter is dismissed.
		/// </summary>
		DismissAll = 0x2000
	}
}
