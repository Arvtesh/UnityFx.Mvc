// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Stores static parameters applicable to <see cref="IAppStateController"/> implementation.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class AppStateControllerAttribute : Attribute
	{
		/// <summary>
		/// Returns the state name. Read only.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Returns state flags. Read only.
		/// </summary>
		public AppStateFlags Flags { get; }

		/// <summary>
		/// Returns index of the state layer. States with higher layers are always displayed over states with lower layer value. Read only.
		/// </summary>
		public int Layer { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="AppStateControllerAttribute"/> class.
		/// </summary>
		public AppStateControllerAttribute(string name, AppStateFlags flags = AppStateFlags.None, int layer = 0)
		{
			Name = name;
			Flags = flags;
			Layer = layer < 0 ? 0 : layer;
		}
	}
}
