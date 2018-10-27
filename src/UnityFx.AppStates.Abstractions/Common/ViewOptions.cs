// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Enumerates view options.
	/// </summary>
	/// <seealso cref="IView"/>
	/// <seealso cref="IViewController"/>
	[Flags]
	public enum ViewOptions
	{
		/// <summary>
		/// Default options.
		/// </summary>
		None = 0,

		/// <summary>
		/// Exclusive views cover all screen hiding all views below. Cannot be mixed with <see cref="Child"/> and <see cref="Transient"/> options.
		/// </summary>
		Exclusive = 1,

		/// <summary>
		/// Child views typically represent a part of a larger view. They do not disable input for the views they do not overlap. Cannot be mixed
		/// with <see cref="Exclusive"/> and <see cref="Transient"/> options.
		/// </summary>
		Child = 2,

		/// <summary>
		/// A short-living popup view that is dismissed by tapping another part of the screen. When active disables input for underlying views.
		/// Cannot be mixed with <see cref="Child"/> and <see cref="Exclusive"/> options.
		/// </summary>
		Transient = 4,
	}
}
