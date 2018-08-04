// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Stores static parameters applicable to state controller implementation.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class AppViewControllerAttribute : Attribute
	{
		/// <summary>
		/// Gets an unique controller (state) identifier that may be used to identify the its type (in deeplinks).
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Gets the controller creation options.
		/// </summary>
		public PresentOptions Options { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="AppViewControllerAttribute"/> class.
		/// </summary>
		public AppViewControllerAttribute(string id, PresentOptions flags = PresentOptions.None)
		{
			Id = id;
			Options = flags;
		}
	}
}
