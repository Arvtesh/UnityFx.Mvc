// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic prefab loader.
	/// </summary>
	public interface IPrefabLoader
	{
		/// <summary>
		/// Loads and instantiates a prefab.
		/// </summary>
		IAsyncOperation<GameObject> LoadPrefab(string prefabId);
	}
}
