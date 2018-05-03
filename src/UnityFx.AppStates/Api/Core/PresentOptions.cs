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
		/// Default options (push). The new state is pushed onto the stack.
		/// </summary>
		None = 0,

		/// <summary>
		/// Pushes new state onto the stack instead of the previous one.
		/// </summary>
		DismissCurrentState = 1,

		/// <summary>
		/// Pushes new state onto the stack instead of all other states.
		/// </summary>
		DismissAllStates = 3,
	}
}
