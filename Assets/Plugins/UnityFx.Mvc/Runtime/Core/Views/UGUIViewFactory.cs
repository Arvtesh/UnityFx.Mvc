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
	/// An UGUI-based view factory.
	/// </summary>
	/// <seealso cref="UGUIViewFactoryBuilder"/>
	internal sealed partial class UGUIViewFactory : MonoBehaviour, IViewFactory
	{
		#region data

		private Dictionary<string, GameObject> _viewPrefabCache;
		private Dictionary<string, Task<GameObject>> _viewPrefabCacheTasks;
		private IReadOnlyList<Transform> _viewRoots;
		private Func<string, Task<GameObject>> _loadPrefabDelegate;
		private Color _popupBgColor;
		private ViewCollection _views;
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Gets popup background color.
		/// </summary>
		public Color PopupBackgroundColor => _popupBgColor;

		/// <summary>
		/// Gets a read-only collection of view root transforms.
		/// </summary>
		public IReadOnlyList<Transform> ViewRoots => _viewRoots;

		/// <summary>
		/// Gets a read-only collection of loaded view prefabs.
		/// </summary>
		public IReadOnlyCollection<string> Prefabs => _viewPrefabCache.Keys;

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

		internal void SetPopupBackgrounColor(Color color)
		{
			_popupBgColor = color;
		}

		internal void SetLoadPrefabDelegate(Func<string, Task<GameObject>> prefabDelegate)
		{
			_loadPrefabDelegate = prefabDelegate;
		}

		internal void SetViewPrefabs(Dictionary<string, GameObject> prefabs)
		{
			_viewPrefabCache = prefabs ?? new Dictionary<string, GameObject>();
		}

		internal void SetViewRoots(IReadOnlyList<Transform> viewRoots)
		{
			if (viewRoots is null || viewRoots.Count == 0)
			{
				var canvases = GetComponentsInChildren<Canvas>();

				if (canvases != null && canvases.Length > 0)
				{
					var transforms = new Transform[canvases.Length];

					for (var i = 0; i < transforms.Length; i++)
					{
						transforms[i] = canvases[i].transform;
					}

					_viewRoots = transforms;
				}
				else
				{
					_viewRoots = new Transform[1] { transform };
				}
			}
			else
			{
				_viewRoots = viewRoots;
			}
		}

		#endregion

		#region MonoBehaviour

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
				throw new ArgumentException("Invalid prefab path.", nameof(prefabPath));
			}

			ViewProxy viewProxy = null;

			try
			{
				var exclusive = (options & PresentOptions.Exclusive) != 0;
				var modal = (options & PresentOptions.Modal) != 0;
				var viewRoot = _viewRoots[layer];

				viewProxy = CreateViewProxy(viewRoot, prefabPath, zIndex, exclusive, modal);

				if (_viewPrefabCache.TryGetValue(prefabPath, out var viewPrefab))
				{
					return CreateView(viewPrefab, viewProxy, parent);
				}
				else
				{
					if (_viewPrefabCacheTasks != null && _viewPrefabCacheTasks.TryGetValue(prefabPath, out var task))
					{
						viewPrefab = await task;
					}
					else if (_loadPrefabDelegate != null)
					{
						task = _loadPrefabDelegate(prefabPath);

						if (_viewPrefabCacheTasks is null)
						{
							_viewPrefabCacheTasks = new Dictionary<string, Task<GameObject>>();
						}

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
					else
					{
						// TODO: Use a dedicated exception type.
						throw new KeyNotFoundException();
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

		private void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		#endregion
	}
}
