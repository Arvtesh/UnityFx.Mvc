// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic container of components.
	/// </summary>
	public interface IComponentContainer
	{
		/// <summary>
		/// Gets a component of the specified type (or <see langword="null"/> if not found).
		/// </summary>
		TComponent GetComponent<TComponent>();

		/// <summary>
		/// Recursively searches for a component of the specified type (returns <see langword="null"/> if not found).
		/// </summary>
		TComponent GetComponentRecursive<TComponent>();

		/// <summary>
		/// Gets all components of the specified type (or <see langword="null"/> if not found).
		/// </summary>
		TComponent[] GetComponents<TComponent>();

		/// <summary>
		/// Recursively searches for components of the specified type (returns <see langword="null"/> if not found).
		/// </summary>
		TComponent[] GetComponentsRecursive<TComponent>();
	}
}
