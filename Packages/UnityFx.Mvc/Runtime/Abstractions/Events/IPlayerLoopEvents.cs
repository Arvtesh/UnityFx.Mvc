// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Defines player loop events.
	/// </summary>
	public interface IPlayerLoopEvents
	{
		/// <summary>
		/// Update event. Called on each frame.
		/// </summary>
		void OnUpdate();

		/// <summary>
		/// LateUpdate event. Called on each frame.
		/// </summary>
		void OnLateUpdate();
	}
}
