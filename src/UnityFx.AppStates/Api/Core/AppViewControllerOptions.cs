// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Enumerates creation options for a <see cref="AppViewController"/> instance.
	/// </summary>
	[Flags]
	public enum AppViewControllerOptions
	{
		/// <summary>
		/// No flags.
		/// </summary>
		None = 0,

		/// <summary>
		/// If set the controller tries to reuse view of its parent controller/state.
		/// </summary>
		ReuseParentView = 1
	}
}
