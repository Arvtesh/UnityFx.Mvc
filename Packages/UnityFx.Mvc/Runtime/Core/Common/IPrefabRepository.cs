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
		/// Loads a prefab with the specified identifier.
		/// </summary>
		/// <param name="key">Resource identifier of the prefab.</param>
		/// <seealso cref="ReleasePrefab(GameObject)"/>
		Task<GameObject> LoadPrefabAsync(object key);

		/// <summary>
		/// Unloads the specified prefab.
		/// </summary>
		/// <param name="resourceId">Resource identifier of the prefab.</param>
		/// <seealso cref="LoadPrefabAsync(object)"/>
		void ReleasePrefab(GameObject prefab);
	}
}
