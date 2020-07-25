// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A generic prefab storage.
	/// </summary>
	public interface IPrefabRepository
	{
		/// <summary>
		/// Gets a read-only collection of cached prefabs.
		/// </summary>
		IDictionary<string, GameObject> PrefabCache { get; }

		/// <summary>
		/// Loads a prefab with the specified identifier and adds it to <see cref="PrefabCache"/>.
		/// </summary>
		/// <param name="resourceId">Resource identifier of the prefab.</param>
		/// <seealso cref="UnloadPrefab(string)"/>
		/// <seealso cref="PrefabCache"/>
		Task<GameObject> LoadPrefabAsync(string resourceId);

		/// <summary>
		/// Unloads the specified prefab and removes it from <see cref="PrefabCache"/>. Does nothing if prefab is not cached.
		/// </summary>
		/// <param name="resourceId">Resource identifier of the prefab.</param>
		/// <seealso cref="LoadPrefabAsync(string)"/>
		/// <seealso cref="PrefabCache"/>
		void UnloadPrefab(string resourceId);
	}
}
