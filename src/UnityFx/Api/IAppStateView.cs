// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFx.App
{
	/// <summary>
	/// A generic view attached to an <see cref="IAppState"/> instance.
	/// </summary>
	/// <seealso cref="IAppState"/>
	public interface IAppStateView : IEnumerable<GameObject>
	{
		/// <summary>
		/// Returns the root <see cref="GameObject"/> of the view (the first one in case of several root game objects). Read only.
		/// </summary>
		GameObject Go { get; }

		/// <summary>
		/// Returns view bounds (in world space) based on its content. Read only.
		/// </summary>
		Bounds Bounds { get; }

		/// <summary>
		/// Enabled or disapbes the view.
		/// </summary>
		bool Enabled { get; set; }

		/// <summary>
		/// Searches the view root for the specified component. Returns <c>null</c> if no components found.
		/// </summary>
		T GetComponent<T>() where T : class;

		/// <summary>
		/// Searches the view root for the specified components. Returns an empty array if no components found.
		/// </summary>
		T[] GetComponents<T>() where T : class;

		/// <summary>
		/// Searches the view for the specified component recursively. Returns <c>null</c> if no components found.
		/// </summary>
		T GetComponentRecursive<T>() where T : class;

		/// <summary>
		/// Searches the view for the specified components recursively. Returns an empty array if no components found.
		/// </summary>
		T[] GetComponentsRecursive<T>() where T : class;

	}
}