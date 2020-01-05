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
		/// Marks the controller as exclusive. Exclusive controllers do not forward unprocessed commands to controllers below them is the stack
		/// and it is assumed their views are full-screen. Cannot be combined with <see cref="Popup"/>.
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
		/// Parents the presented controller to the caller. Child controllers are dismissed with their parent controllers.
		/// </summary>
		Child = 0x10,

		/// <summary>
		/// If set the caller presenter is dismissed.
		/// </summary>
		DismissCurrent = 0x1000,

		/// <summary>
		/// If set all controllers are dismissed before presenting the new one.
		/// </summary>
		DismissAll = 0x2000
	}
}
