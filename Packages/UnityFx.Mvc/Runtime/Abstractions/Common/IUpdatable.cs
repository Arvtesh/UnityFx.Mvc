// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A target for UPDATE notifications.
	/// </summary>
	public interface IUpdatable
	{
		/// <summary>
		/// Called on each frame.
		/// </summary>
		void Update(float frameTime);
	}
}
