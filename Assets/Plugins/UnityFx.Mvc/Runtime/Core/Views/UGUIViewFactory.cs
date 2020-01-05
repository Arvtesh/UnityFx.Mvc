// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Default <see cref="MonoBehaviour"/>-based view factory.
	/// </summary>
	public partial class UGUIViewFactory : MonoBehaviour, IViewFactory
	{
		#region data

		[SerializeField]
		private Color _popupBgColor = new Color(0, 0, 0, 0.5f);
		[SerializeField]
		private Transform[] _viewRoots;
		[SerializeField]
		private GameObject[] _viewPrefabs;

		private Dictionary<string, GameObject> _viewPrefabCache = new Dictionary<string, GameObject>();
		private Dictionary<string, Task<GameObject>> _viewPrefabCacheTasks = new Dictionary<string, Task<GameObject>>();
		private ViewCollection _views;
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Gets background color of the popup views.
		/// </summary>
		public Color PopupBackgroundColor { get => _popupBgColor; set => _popupBgColor = value; }

		/// <summary>
		/// Gets or sets list of preloaded view prefabs.
		/// </summary>
		public GameObject[] ViewPrefabs { get => _viewPrefabs; set => _viewPrefabs = value; }

		/// <summary>
		/// Gets or sets list of preloaded view prefabs.
		/// </summary>
		public Transform[] ViewRoots { get => _viewRoots; set => _viewRoots = value; }

		/// <summary>
		/// Gets a read-only collection of views.
		/// </summary>
		public ViewCollection Views
		{
			get
			{
				if (_views is null)
				{
					_views = new ViewCollection(this);
				}

				return _views;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the instance is disposed.
		/// </summary>
		protected bool IsDisposed => _disposed;

		/// <summary>
		/// Clears the prefabs cache.
		/// </summary>
		protected void ClearPrefabCache()
		{
			_viewPrefabCache?.Clear();
		}

		/// <summary>
		/// Gets root <see cref="Transform"/> for a view of the specified controller.
		/// </summary>
		protected virtual Transform GetViewRoot(int layer)
		{
			if (_viewRoots is null || _viewRoots.Length == 0)
			{
				return transform;
			}

			return _viewRoots[layer];
		}

		/// <summary>
		/// Loads view prefab with the specified name. Default implementation searches the prefab in <see cref="ViewPrefabs"/> array, returns <see langword="null"/> on any error.
		/// Overide to implement own mechanism of loading views.
		/// </summary>
		protected virtual Task<GameObject> LoadViewPrefabAsync(string prefabName)
		{
			if (_viewPrefabs != null && !string.IsNullOrEmpty(prefabName))
			{
				foreach (var go in _viewPrefabs)
				{
					if (string.CompareOrdinal(go.name, prefabName) == 0)
					{
						return Task.FromResult(go);
					}
				}
			}

			return Task.FromResult(default(GameObject));
		}

		/// <summary>
		/// Throws an <see cref="ObjectDisposedException"/> if the controller is disposed.
		/// </summary>
		/// <seealso cref="Dispose"/>
		protected void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		/// <summary>
		/// Called when the view is disposed.
		/// </summary>
		/// <seealso cref="ThrowIfDisposed"/>
		protected virtual void OnDispose()
		{
		}

		#endregion

		#region MonoBehaviour

#if UNITY_EDITOR

		protected virtual void Reset()
		{
			var cs = GetComponentsInChildren<Canvas>();

			_viewRoots = new Transform[cs.Length];

			for (var i = 0; i < cs.Length; i++)
			{
				_viewRoots[i] = cs[i].transform;
			}
		}

#endif

		private void OnDestroy()
		{
			Dispose();
		}

		#endregion

		#region IViewFactory

		public async Task<IView> CreateAsync(string prefabPath, int layer, int zIndex, PresentOptions options, Transform parent)
		{
			ThrowIfDisposed();

			if (prefabPath is null)
			{
				throw new ArgumentNullException(nameof(prefabPath));
			}

			if (string.IsNullOrWhiteSpace(prefabPath))
			{
				throw new ArgumentException("Invalid prefab name.", nameof(prefabPath));
			}

			ViewProxy viewProxy = null;

			try
			{
				var exclusive = (options & PresentOptions.Exclusive) != 0;
				var modal = (options & PresentOptions.Modal) != 0;
				var viewRoot = GetViewRoot(layer);

				viewProxy = CreateViewProxy(viewRoot, prefabPath, zIndex, exclusive, modal);

				if (_viewPrefabCache.TryGetValue(prefabPath, out var viewPrefab))
				{
					return CreateView(viewPrefab, viewProxy, parent);
				}
				else
				{
					if (_viewPrefabCacheTasks.TryGetValue(prefabPath, out var task))
					{
						viewPrefab = await task;
					}
					else
					{
						task = LoadViewPrefabAsync(prefabPath);
						_viewPrefabCacheTasks.Add(prefabPath, task);

						try
						{
							viewPrefab = await task;
						}
						finally
						{
							_viewPrefabCacheTasks.Remove(prefabPath);
						}
					}

					if (_disposed)
					{
						throw new OperationCanceledException();
					}

					_viewPrefabCache.Add(prefabPath, viewPrefab);
					return CreateView(viewPrefab, viewProxy, parent);
				}
			}
			catch
			{
				if (viewProxy)
				{
					Destroy(viewProxy.gameObject);
				}

				throw;
			}
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				_viewPrefabCache = null;

				OnDispose();
			}
		}

		#endregion

		#region implementation

		private IView CreateView(GameObject viewPrefab, ViewProxy viewProxy, Transform parent)
		{
			Debug.Assert(viewProxy);

			GameObject go;
			IView view;

			if (viewPrefab)
			{
				go = Instantiate(viewPrefab, parent ?? viewProxy.transform, false);
				view = go.GetComponent<IView>();

				if (view == null)
				{
					view = go.AddComponent<View>();
				}
			}
			else
			{
				go = new GameObject(viewProxy.name);
				view = go.AddComponent<View>();

				go.transform.SetParent(parent ?? viewProxy.transform);
			}

			viewProxy.View = view;
			return view;
		}

		private ViewProxy CreateViewProxy(Transform viewRoot, string viewName, int zIndex, bool exclusive, bool modal)
		{
			Debug.Assert(viewName != null);

			var go = new GameObject(viewName);
			var viewProxy = go.AddComponent<ViewProxy>();

			go.transform.SetParent(viewRoot, false);

			if (zIndex < 0)
			{
				go.transform.SetAsFirstSibling();
			}
			else if (zIndex < viewRoot.childCount)
			{
				go.transform.SetSiblingIndex(zIndex);
			}

			if (modal)
			{
				var image = go.AddComponent<Image>();
				var rt = image.rectTransform;

				rt.anchorMin = Vector2.zero;
				rt.anchorMax = Vector2.one;
				rt.offsetMin = Vector2.zero;
				rt.offsetMax = Vector2.zero;

				image.color = _popupBgColor;
				viewProxy.Image = image;
			}
			else
			{
				var rt = go.AddComponent<RectTransform>();

				rt.anchorMin = Vector2.zero;
				rt.anchorMax = Vector2.one;
				rt.offsetMin = Vector2.zero;
				rt.offsetMax = Vector2.zero;
			}

			viewProxy.Modal = modal;
			viewProxy.Exclusive = exclusive;

			return viewProxy;
		}

		private void UpdatePopupBackgrounds()
		{
			var modalFound = false;

			for (var i = transform.childCount - 1; i >= 0; --i)
			{
				var c = transform.GetChild(i).GetComponent<ViewProxy>();

				if (c && c.Image)
				{
					if (modalFound)
					{
						c.Image.enabled = false;
					}
					else
					{
						modalFound = c.Modal;
						c.Image.enabled = modalFound;
					}
				}
			}
		}

		private void ThrowIfInvalidPrefab(GameObject prefabGo)
		{
			if (!prefabGo)
			{
				throw new OperationCanceledException();
			}
		}

		#endregion
	}
}
