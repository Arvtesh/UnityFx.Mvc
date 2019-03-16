// Copyright (c) Alexander Bogarsukov.
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
		/// Marks the controller as modal. Modal controllers do not forward unprocessed commands to controllers below them is the stack.
		/// </summary>
		Modal = 1,

		/// <summary>
		/// Dismisses the caller before presenting.
		/// </summary>
		DismissCurrent = 0x00000100,

		/// <summary>
		/// Dismisses all controllers before presenting.
		/// </summary>
		DismissAll = 0x00000200,

		/// <summary>
		/// When presented, the target controller will not be activated.
		/// </summary>
		DoNotActivate = 0x00010000,
	}
}
