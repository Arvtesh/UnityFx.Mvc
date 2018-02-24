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
		/// Gets an unique controller (state) identifier that may be used to identify the its type (in deeplinks).
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Gets controller (state) flags.
		/// </summary>
		public AppStateFlags Flags { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="AppStateControllerAttribute"/> class.
		/// </summary>
		public AppStateControllerAttribute(string id, AppStateFlags flags = AppStateFlags.None)
		{
			Id = id;
			Flags = flags;
		}
	}
}
