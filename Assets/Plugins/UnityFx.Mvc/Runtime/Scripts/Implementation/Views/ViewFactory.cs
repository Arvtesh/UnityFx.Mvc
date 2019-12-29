// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Default <see cref="MonoBehaviour"/>-based view factory.
	/// </summary>
	public partial class ViewFactory : MonoBehaviour, IViewFactory, IContainer
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
		private ComponentCollection _components;
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
		protected virtual Transform GetViewRoot(Type controllerType, ViewControllerAttribute attr)
		{
			if (_viewRoots is null || _viewRoots.Length == 0)
			{
				return transform;
			}

			return _viewRoots[attr.ViewLayer];
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

		public async Task<IView> CreateAsync(Type controllerType, int zIndex, PresentOptions options, Transform parent)
		{
			ThrowIfDisposed();

			if (controllerType is null)
			{
				throw new ArgumentNullException(nameof(controllerType));
			}

			ViewProxy viewProxy = null;

			try
			{
				var attrs = (ViewControllerAttribute[])controllerType.GetCustomAttributes(typeof(ViewControllerAttribute), false);
				var attr = attrs != null && attrs.Length > 0 ? attrs[0] : null;
				var exclusive = (options & PresentOptions.Exclusive) != 0;
				var modal = (options & PresentOptions.Modal) != 0;
				var prefabName = GetPrefabName(controllerType, attr);
				var viewRoot = GetViewRoot(controllerType, attr);

				viewProxy = CreateViewProxy(viewRoot, prefabName, zIndex, exclusive, modal);

				if (_viewPrefabCache.TryGetValue(prefabName, out var viewPrefab))
				{
					return CreateView(viewPrefab, viewProxy, parent);
				}
				else
				{
					if (_viewPrefabCacheTasks.TryGetValue(prefabName, out var task))
					{
						viewPrefab = await task;
					}
					else
					{
						task = LoadViewPrefabAsync(prefabName);
						_viewPrefabCacheTasks.Add(prefabName, task);

						try
						{
							viewPrefab = await task;
						}
						finally
						{
							_viewPrefabCacheTasks.Remove(prefabName);
						}
					}

					if (_disposed)
					{
						throw new OperationCanceledException();
					}

					_viewPrefabCache.Add(prefabName, viewPrefab);
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

		#region IContainer

		ComponentCollection IContainer.Components
		{
			get
			{
				ThrowIfDisposed();

				if (_components is null)
				{
					_components = new ComponentCollection(Views.ToArray());
				}

				return _components;
			}
		}

		void IContainer.Add(IComponent component)
		{
			ThrowIfDisposed();

			if (component is null)
			{
				throw new ArgumentNullException(nameof(component));
			}

			AddInternal(component, null);
		}

		void IContainer.Add(IComponent component, string name)
		{
			ThrowIfDisposed();

			if (component is null)
			{
				throw new ArgumentNullException(nameof(component));
			}

			AddInternal(component, name);
		}

		void IContainer.Remove(IComponent component)
		{
			if (component != null && !_disposed)
			{
				_components = null;

				if (component.Site != null)
				{
					if (!ReferenceEquals(component.Site.Container, this))
					{
						return;
					}

					if (component.Site is ViewProxy p)
					{
						p.Container = null;
						Destroy(p.gameObject);
					}

					component.Site = null;
				}

				UpdatePopupBackgrounds();
			}
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				_components = null;
				_viewPrefabCache = null;

				OnDispose();
			}
		}

		#endregion

		#region implementation

		private void AddInternal(IComponent component, string name)
		{
			Debug.Assert(component != null);

			var view = component as IView;

			if (view is null)
			{
				throw new ArgumentException("The component should implement IView.", nameof(component));
			}

			if (string.IsNullOrEmpty(name))
			{
				name = "View";
			}

			var viewProxy = CreateViewProxy(null, name, transform.childCount, false, false);
			viewProxy.Component = component;
		}

		private string GetPrefabName(Type controllerType, ViewControllerAttribute attr)
		{
			Debug.Assert(controllerType != null);

			if (attr != null && !string.IsNullOrEmpty(attr.ViewPrefabName))
			{
				return attr.ViewPrefabName;
			}
			else
			{
				var viewName = controllerType.Name;

				if (viewName.EndsWith("Controller"))
				{
					viewName = viewName.Substring(0, viewName.Length - 10);
				}

				return viewName;
			}
		}

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

			viewProxy.Component = view;
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
			viewProxy.Container = this;
			viewProxy.Name = viewName;

			return viewProxy;
		}

		private void UpdatePopupBackgrounds()
		{
			var modalFound = false;

			for (var i = transform.childCount - 1; i >= 0; --i)
			{
				var c = transform.GetChild(i).GetComponent<ViewProxy>();

				if (c && c.Container != null && c.Image)
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
