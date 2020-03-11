// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc.Extensions
{
	/// <summary>
	/// Enumerates commands of <see cref="MessageBoxController"/>.
	/// </summary>
	/// <seealso cref="MessageBoxOptions"/>
	/// <seealso cref="MessageBoxController"/>
	public enum MessageBoxCommands
	{
		/// <summary>
		/// OK button was pressed.
		/// </summary>
		Ok,

		/// <summary>
		/// CANCEL button was pressed.
		/// </summary>
		Cancel,

		/// <summary>
		/// CLOSE button was pressed.
		/// </summary>
		Close
	}
}
