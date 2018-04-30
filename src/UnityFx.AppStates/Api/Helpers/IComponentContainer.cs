// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// tt
	/// </summary>
	public interface IComponentContainer
	{
		/// <summary>
		/// tt
		/// </summary>
		TComponent GetComponent<TComponent>();

		/// <summary>
		/// tt
		/// </summary>
		TComponent GetComponentRecursive<TComponent>();

		/// <summary>
		/// tt
		/// </summary>
		TComponent[] GetComponents<TComponent>();

		/// <summary>
		/// tt
		/// </summary>
		TComponent[] GetComponentsRecursive<TComponent>();
	}
}
