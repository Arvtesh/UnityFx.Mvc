// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Stores default options value for <see cref="IViewController"/> implementations.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ViewControllerOptionsAttribute : Attribute
	{
		/// <summary>
		/// Gets the default controller creation options.
		/// </summary>
		public PresentOptions Options { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewControllerOptionsAttribute"/> class.
		/// </summary>
		public ViewControllerOptionsAttribute(PresentOptions options)
		{
			Options = options;
		}
	}
}
