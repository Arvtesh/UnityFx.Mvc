// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	internal class SimplePrefabRepository : IPrefabRepository, IDisposable
	{
		#region data

		private readonly Dictionary<object, GameObject> _prefabs = new Dictionary<object, GameObject>();
		private bool _disposed;

		#endregion

		#region interface

		public void AddPrefab(object key, GameObject prefab)
		{
			_prefabs.Add(key, prefab);
		}

		#endregion

		#region IPrefabRepository

		public Task<GameObject> LoadPrefabAsync(object key)
		{
			if (key is null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			if (_prefabs.TryGetValue(key, out var prefab))
			{
				return Task.FromResult(prefab);
			}

			throw new KeyNotFoundException();
		}

		public void ReleasePrefab(GameObject prefab)
		{
			// Do nothing.
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
