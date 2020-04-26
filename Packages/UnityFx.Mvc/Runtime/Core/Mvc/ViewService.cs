// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A <see cref="MonoBehaviour"/>-based implementation of <see cref="IViewService"/>.
	/// </summary>
	public abstract class ViewService : MonoBehaviour, IViewService
	{
		#region data

		private Dictionary<string, GameObject> _viewPrefabCache;
		private Dictionary<string, Task<GameObject>> _viewPrefabCacheTasks;
		private IReadOnlyList<Transform> _layers;
		private IReadOnlyCollection<IView> _views;
		private Func<string, Task<GameObject>> _loadPrefabDelegate;
		private Color _popupBgColor;
		private bool _disposed;

		#endregion

		#region interface

		internal void SetPopupBackgrounColor(Color color)
		{
			_popupBgColor = color;
		}

		internal void SetLoadPrefabDelegate(Func<string, Task<GameObject>> prefabDelegate)
		{
			Debug.Assert(_loadPrefabDelegate is null);

			_loadPrefabDelegate = prefabDelegate;
		}

		internal void SetViewPrefabs(Dictionary<string, GameObject> prefabs)
		{
			Debug.Assert(_viewPrefabCache is null);

			_viewPrefabCache = prefabs ?? new Dictionary<string, GameObject>();
		}

		internal void SetLayers(IReadOnlyList<Transform> layers)
		{
			Debug.Assert(_layers is null);

			if (layers is null || layers.Count == 0)
			{
				var canvases = GetComponentsInChildren<Canvas>();

				if (canvases != null && canvases.Length > 0)
				{
					var transforms = new Transform[canvases.Length];

					for (var i = 0; i < transforms.Length; i++)
					{
						transforms[i] = canvases[i].transform;
					}

					_layers = transforms;
				}
				else
				{
					_layers = new Transform[1] { transform };
				}
			}
			else
			{
				_layers = layers;
			}
		}

		/// <summary>
		/// Creates a view collection.
		/// </summary>
		/// <seealso cref="Views"/>
		protected abstract IReadOnlyCollection<IView> CreateViewCollection();

		/// <summary>
		/// Dispose handler.
		/// </summary>
		/// <seealso cref="Dispose"/>
		protected virtual void OnDispose()
		{
		}

		/// <summary>
		/// Throws <see cref="ObjectDisposedException"/> is the service is disposed.
		/// </summary>
		protected void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		#endregion

		#region MonoBehaviour

		private void OnDestroy()
		{
			Dispose();
		}

		#endregion

		#region IViewService

		public Color PopupBackgroundColor => _popupBgColor;

		public IReadOnlyList<Transform> Layers => _layers;

		public IReadOnlyDictionary<string, GameObject> Prefabs => _viewPrefabCache;

		public IReadOnlyCollection<IView> Views
		{
			get
			{
				if (_views is null)
				{
					_views = CreateViewCollection();
				}

				return _views;
			}
		}

		#endregion

		#region IViewFactory

		public async Task<GameObject> LoadViewPrefabAsync(string resourceId)
		{
			ThrowIfDisposed();

			if (resourceId is null)
			{
				throw new ArgumentNullException(nameof(resourceId));
			}

			if (string.IsNullOrWhiteSpace(resourceId))
			{
				throw new ArgumentException(Messages.Format_InvalidPrefabPath(), nameof(resourceId));
			}

			if (_viewPrefabCache.TryGetValue(resourceId, out var prefab))
			{
				return prefab;
			}

			if (_viewPrefabCacheTasks != null && _viewPrefabCacheTasks.TryGetValue(resourceId, out var task))
			{
				prefab = await task;
			}
			else if (_loadPrefabDelegate != null)
			{
				task = _loadPrefabDelegate(resourceId);

				if (_viewPrefabCacheTasks is null)
				{
					_viewPrefabCacheTasks = new Dictionary<string, Task<GameObject>>();
				}

				_viewPrefabCacheTasks.Add(resourceId, task);

				try
				{
					prefab = await task;
				}
				finally
				{
					_viewPrefabCacheTasks.Remove(resourceId);
				}
			}
			else
			{
				throw new InvalidOperationException(Messages.Format_PrefabCannotBeLoaded(resourceId));
			}

			if (_disposed)
			{
				Destroy(prefab);
				throw new OperationCanceledException();
			}

			_viewPrefabCache.Add(resourceId, prefab);
			return prefab;
		}

		public abstract Task<IView> PresentViewAsync(string resourceId, int layer, int zIndex, ViewControllerFlags flags, Transform parent);

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				OnDispose();
			}
		}

		#endregion
	}
}
