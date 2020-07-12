// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A prefab repository.
	/// </summary>
	public interface IPrefabRepository : IDisposable
	{
		/// <summary>
		/// Gets a read-only collection of cached prefabs.
		/// </summary>
		IDictionary<string, GameObject> Prefabs { get; }

		/// <summary>
		/// Loads prefab with the specified identifier.
		/// </summary>
		/// <param name="resourceId">Resource identifier of the prefab.</param>
		/// <seealso cref="UnloadPrefab(string)"/>
		Task<GameObject> LoadPrefabAsync(string resourceId);

		/// <summary>
		/// Unloads the specified prefab. It can be loaded back with <see cref="LoadPrefabAsync(string)"/>.
		/// </summary>
		/// <param name="resourceId">Resource identifier of the prefab.</param>
		/// <seealso cref="LoadPrefabAsync(string)"/>
		void UnloadPrefab(string resourceId);
	}
}
