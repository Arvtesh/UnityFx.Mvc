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
		/// Makes sure the present call return immediately after queueing the operation.
		/// </summary>
		ExcecuteAsync = 1,

		/// <summary>
		/// Marks the controller as modal. Modal controllers do not forward unprocessed commands to controllers below them is the stack.
		/// </summary>
		Modal = 2,

		/// <summary>
		/// Presents a new state and dismisses the previous one.
		/// </summary>
		DismissCurrentController = 0x00001000,

		/// <summary>
		/// Presents a new state and dismisses all other states.
		/// </summary>
		DismissAllStates = 0x00002000,

		/// <summary>
		/// When presented, the target controller will not be activated.
		/// </summary>
		DoNotActivate = 0x00010000,
	}
}
