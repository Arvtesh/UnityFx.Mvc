// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// An object with instance identifier.
	/// </summary>
	public interface IObjectId
	{
		/// <summary>
		/// Gets an object instance identifier.
		/// </summary>
		int Id { get; }

		/// <summary>
		/// Gets or sets the identifying name of the object. It is not required to be unique.
		/// </summary>
		string Name { get; set; }
	}
}
