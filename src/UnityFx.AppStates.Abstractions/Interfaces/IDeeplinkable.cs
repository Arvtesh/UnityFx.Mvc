// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A object that can be target for a deeplink.
	/// </summary>
	public interface IDeeplinkable
	{
		/// <summary>
		/// Gets the deeplink identifier assosiated with this type.
		/// </summary>
		string DeeplinkId { get; }
	}
}
