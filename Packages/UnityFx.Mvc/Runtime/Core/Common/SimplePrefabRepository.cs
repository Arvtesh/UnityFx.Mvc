// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	internal class SimplePrefabRepository : IPrefabRepository
	{
		#region data

		private readonly Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();
		private bool _disposed;

		#endregion

		#region IPrefabRepository

		public IDictionary<string, GameObject> Prefabs => _prefabs;

		public Task<GameObject> LoadPrefabAsync(string resourceId)
		{
			if (resourceId is null)
			{
				throw new ArgumentNullException(nameof(resourceId));
			}

			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			if (_prefabs.TryGetValue(resourceId, out var prefab))
			{
				return Task.FromResult(prefab);
			}

			throw new KeyNotFoundException();
		}

		public void UnloadPrefab(string resourceId)
		{
			if (resourceId is null)
			{
				return;
			}

			if (_prefabs.TryGetValue(resourceId, out var prefab))
			{
				GameObject.Destroy(prefab);
				_prefabs.Remove(resourceId);
			}
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;

				foreach (var prefab in _prefabs.Values)
				{
					GameObject.Destroy(prefab);
				}
			}
		}

		#endregion
	}
}
