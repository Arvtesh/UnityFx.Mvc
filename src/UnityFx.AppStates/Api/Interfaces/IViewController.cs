// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic view controller.
	/// </summary>
	public interface IViewController
	{
		/// <summary>
		/// Gets the controller identifier.
		/// </summary>
		string Id { get; }
	}
}
