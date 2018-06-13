// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Enumerates state controller push options.
	/// </summary>
	[Flags]
	public enum PresentOptions
	{
		/// <summary>
		/// Default options. The new state is pushed onto the stack.
		/// </summary>
		None = 0,

		/// <summary>
		/// tt
		/// </summary>
		Child = 1,

		/// <summary>
		/// Presents a new state and dismisses the previous one.
		/// </summary>
		DismissCurrentState = 0x00001000,

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
