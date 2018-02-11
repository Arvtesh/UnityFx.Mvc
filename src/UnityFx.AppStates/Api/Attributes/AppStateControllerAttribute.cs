// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Stores static parameters applicable to state controller implementation.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class AppStateControllerAttribute : Attribute
	{
		/// <summary>
		/// Returns the state name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Returns state flags.
		/// </summary>
		public AppStateFlags Flags { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="AppStateControllerAttribute"/> class.
		/// </summary>
		public AppStateControllerAttribute(string name, AppStateFlags flags = AppStateFlags.None)
		{
			Name = name;
			Flags = flags;
		}
	}
}
