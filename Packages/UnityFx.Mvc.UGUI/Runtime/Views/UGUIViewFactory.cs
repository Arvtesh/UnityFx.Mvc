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
	internal sealed partial class UGUIViewFactory : MonoBehaviour, IViewService
	{
		#region data

		private Dictionary<string, GameObject> _viewPrefabCache;
		private Dictionary<string, Task<GameObject>> _viewPrefabCacheTasks;
		private IReadOnlyList<Transform> _layers;
		private Func<string, Task<GameObject>> _loadPrefabDelegate;
		private Color _popupBgColor;
		private ViewCollection _views;
		private bool _disposed;

		#endregion

		#region interface

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

					_layers = transforms;
				}
				else
				{
					_layers = new Transform[1] { transform };
				}
			}
			else
			{
				_layers = viewRoots;
			}
		}

		internal void OnDestroyView(UGUIViewProxy view)
		{
			if (view)
			{
				UpdateViewStack(view.transform.parent);
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
					_views = new ViewCollection(this);
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

		public async Task<IView> PresentViewAsync(string resourceId, int layer, int zIndex, ViewControllerFlags flags, Transform parent)
		{
			UGUIViewProxy viewProxy = null;

			try
			{
				var exclusive = (flags & ViewControllerFlags.Exclusive) != 0;
				var modal = (flags & ViewControllerFlags.Modal) != 0;
				var viewRoot = _layers[layer];
				var viewPrefab = await LoadViewPrefabAsync(resourceId);

				viewProxy = CreateViewProxy(viewRoot, resourceId, zIndex, exclusive, modal);

				return CreateView(resourceId, viewPrefab, viewProxy, parent);
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

		private IView CreateView(string resourceId, GameObject prefab, UGUIViewProxy viewProxy, Transform parent)
		{
			Debug.Assert(viewProxy);

			GameObject go;
			UGUIView view;

			if (prefab)
			{
				view = MvcUtilities.InstantiateViewPrefab<UGUIView>(prefab, parent ?? viewProxy.transform, resourceId);
				go = view.gameObject;
			}
			else
			{
				go = new GameObject(viewProxy.name);
				view = go.AddComponent<UGUIView>();

				go.transform.SetParent(parent ?? viewProxy.transform);
			}

			viewProxy.View = view;

			UpdateViewStack(go.transform.parent);
			return view;
		}

		private UGUIViewProxy CreateViewProxy(Transform viewRoot, string viewName, int zIndex, bool exclusive, bool modal)
		{
			Debug.Assert(viewName != null);

			var go = new GameObject(viewName);
			var viewProxy = go.AddComponent<UGUIViewProxy>();

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

			viewProxy.Factory = this;
			viewProxy.Modal = modal;
			viewProxy.Exclusive = exclusive;

			return viewProxy;
		}

		private void UpdateViewStack(Transform parent)
		{
			if (parent)
			{
				var modalFound = false;

				for (var i = parent.childCount - 1; i >= 0; --i)
				{
					var t = parent.GetChild(i);

					if (t)
					{
						var c = t.GetComponent<UGUIViewProxy>();

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
