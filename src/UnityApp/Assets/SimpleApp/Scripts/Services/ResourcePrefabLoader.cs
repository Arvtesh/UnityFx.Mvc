// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A simple prefab loader that uses <see cref="Resources"/> as the prfab source.
	/// </summary>
	public class ResourcePrefabLoader : IPrefabLoader
	{
		/// <inheritdoc/>
		public IAsyncOperation<GameObject> LoadPrefab(string prefabId)
		{
			return Resources.LoadAsync(prefabId).ToAsync<GameObject>();
		}
	}
}
