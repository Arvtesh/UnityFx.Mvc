﻿// Copyright (c) Alexander Bogarsukov.
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
		private Transform _viewRootTransform;
		[SerializeField]
		private Color _popupBgColor = new Color(0, 0, 0, 0.5f);
		[SerializeField]
		private GameObject[] _viewPrefabs;

		private Dictionary<string, GameObject> _viewPrefabCache = new Dictionary<string, GameObject>();
		private Dictionary<string, Task<GameObject>> _viewPrefabCacheTasks = new Dictionary<string, Task<GameObject>>();
		private ComponentCollection _components;
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Gets root transform for the created views.
		/// </summary>
		public Transform ViewRootTransform
		{
			get
			{
				if (_viewRootTransform is null)
				{
					_viewRootTransform = transform;
				}

				return _viewRootTransform;
			}
			set
			{
				ThrowIfDisposed();

				if (value is null)
				{
					value = transform;
				}

				if (_viewRootTransform != value)
				{
					DestroyAllViews();
					_viewRootTransform = value;
				}
			}
		}

		/// <summary>
		/// Gets background color of the popup views.
		/// </summary>
		public Color PopupBackgroundColor { get => _popupBgColor; set => _popupBgColor = value; }

		/// <summary>
		/// Gets or sets list of preloaded view prefabs.
		/// </summary>
		public GameObject[] ViewPrefabs { get => _viewPrefabs; set => _viewPrefabs = value; }

		/// <summary>
		/// gets a value indicating whether the instance is disposed.
		/// </summary>
		protected bool IsDisposed => _disposed;

		/// <summary>
		/// Gets all views.
		/// </summary>
		protected IView[] GetViews()
		{
			var viewRoot = ViewRootTransform;
			var result = new IView[viewRoot.childCount];

			for (var i = 0; i < viewRoot.childCount; ++i)
			{
				var proxy = viewRoot.GetChild(i).GetComponent<ViewProxy>();

				if (proxy)
				{
					result[i] = proxy.Component as IView;
				}
			}

			return result;
		}

		/// <summary>
		/// Clears the prefabs cache.
		/// </summary>
		protected void ClearPrefabCache()
		{
			_viewPrefabCache?.Clear();
		}

		/// <summary>
		/// Loads view prefab with the specified name. Default implementation searches the prefab in <see cref="ViewPrefabs"/> array. Returns <see langword="null"/> on any error.
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

		private void OnDestroy()
		{
			Dispose();
		}

		#endregion

		#region IViewFactory

		public async Task<IView> CreateViewAsync(Type controllerType, int zIndex, Transform parent)
		{
			ThrowIfDisposed();

			if (controllerType is null)
			{
				throw new ArgumentNullException(nameof(controllerType));
			}

			ViewProxy viewProxy = null;

			try
			{
				var prefabName = GetPrefabName(controllerType);

				viewProxy = CreateViewProxy(prefabName, zIndex, false);

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

		public ComponentCollection Components
		{
			get
			{
				ThrowIfDisposed();

				if (_components is null)
				{
					_components = new ComponentCollection(GetViews());
				}

				return _components;
			}
		}

		public void Add(IComponent component)
		{
			Add(component, null);
		}

		public void Add(IComponent component, string name)
		{
			ThrowIfDisposed();

			if (component is null)
			{
				throw new ArgumentNullException(nameof(component));
			}

			var view = component as IView;

			if (view is null)
			{
				throw new ArgumentException("The component should implement IView.", nameof(component));
			}

			if (string.IsNullOrEmpty(name))
			{
				name = "View";
			}

			var viewProxy = CreateViewProxy(name, ViewRootTransform.childCount, false);
			viewProxy.Component = component;
		}

		public void Remove(IComponent component)
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

				DestroyAllViews();
				OnDispose();
			}
		}

		#endregion

		#region implementation

		private string GetPrefabName(Type controllerType)
		{
			Debug.Assert(controllerType != null);

			var attrs = (ViewControllerAttribute[])controllerType.GetCustomAttributes(typeof(ViewControllerAttribute), false);
			var attr = attrs != null && attrs.Length > 0 ? attrs[0] : null;

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

		private ViewProxy CreateViewProxy(string viewName, int zIndex, bool exclusive)
		{
			Debug.Assert(viewName != null);

			var go = new GameObject(viewName);
			var viewProxy = go.AddComponent<ViewProxy>();
			var viewRoot = ViewRootTransform;

			go.transform.SetParent(viewRoot, false);

			if (zIndex < 0)
			{
				go.transform.SetAsFirstSibling();
			}
			else if (zIndex < viewRoot.childCount)
			{
				go.transform.SetSiblingIndex(zIndex);
			}

			if (viewProxy.transform is RectTransform && !exclusive)
			{
				var image = go.AddComponent<Image>();

				image.color = _popupBgColor;
				image.rectTransform.anchorMin = Vector2.zero;
				image.rectTransform.anchorMax = Vector2.one;
				image.rectTransform.offsetMin = Vector2.zero;
				image.rectTransform.offsetMax = Vector2.zero;

				viewProxy.Image = image;
			}

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
						modalFound = c.Exclusive;
						c.Image.enabled = modalFound;
					}
				}
			}
		}

		private void DestroyAllViews()
		{
			var viewRoot = ViewRootTransform;

			for (var i = 0; i < viewRoot.childCount; ++i)
			{
				var p = viewRoot.GetChild(i).GetComponent<ViewProxy>();

				if (p)
				{
					p.DestroyInternal();
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
