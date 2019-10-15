// Copyright (c) Alexander Bogarsukov.
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
	/// Default implementatino of <see cref="IViewFactory"/>.
	/// </summary>
	public abstract class ViewFactory : MonoBehaviour, IViewFactory, IContainer
	{
		#region data

		[SerializeField]
		private Transform _viewRootTransform;
		[SerializeField]
		private Color _popupBgColor = new Color(0, 0, 0, 0.5f);

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
				return _viewRootTransform;
			}
			set
			{
				ThrowIfDisposed();

				if (value == null)
				{
					throw new ArgumentNullException(nameof(ViewRootTransform));
				}

				if (_viewRootTransform != value)
				{
					if (_viewRootTransform != null)
					{
						for (var i = 0; i < _viewRootTransform.childCount; ++i)
						{
							Destroy(_viewRootTransform.GetChild(i));
						}
					}

					_viewRootTransform = value;
				}
			}
		}

		/// <summary>
		/// Gets background color of the popup views.
		/// </summary>
		public Color PopupBackgroundColor { get => _popupBgColor; set => _popupBgColor = value; }

		/// <summary>
		/// gets a value indicating whether the instance is disposed.
		/// </summary>
		protected bool IsDisposed => _disposed;

		/// <summary>
		/// Gets all views.
		/// </summary>
		protected IView[] GetViews()
		{
			if (_viewRootTransform)
			{
				var result = new IView[_viewRootTransform.childCount];

				for (var i = 0; i < _viewRootTransform.childCount; ++i)
				{
					var proxy = _viewRootTransform.GetChild(i).GetComponent<ViewProxy>();

					if (proxy)
					{
						result[i] = proxy.Component as IView;
					}
				}

				return result;
			}

			return Array.Empty<IView>();
		}

		/// <summary>
		/// Initiates loading view prefab.
		/// </summary>
		protected abstract Task<GameObject> LoadViewPrefabAsync(string prefabName);

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
			GameObject viewPrefab;

			try
			{
				var prefabName = controllerType.Name;

				viewProxy = CreateViewProxy(prefabName, zIndex, false);
				viewPrefab = await LoadViewPrefabAsync(prefabName);

				if (_disposed)
				{
					throw new OperationCanceledException();
				}

				if (!viewPrefab)
				{
					throw new OperationCanceledException();
				}

				return CreateView(viewPrefab, viewProxy, parent ?? viewProxy.transform);
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
			throw new NotImplementedException();
		}

		public void Add(IComponent component, string name)
		{
			throw new NotImplementedException();
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

				if (_viewRootTransform != null)
				{
					for (var i = 0; i < _viewRootTransform.childCount; ++i)
					{
						Destroy(_viewRootTransform.GetChild(i));
					}
				}

				OnDispose();
			}
		}

		#endregion

		#region implementation

		private ViewProxy CreateViewProxy(string viewName, int zIndex, bool exclusive)
		{
			Debug.Assert(viewName != null);

			var go = new GameObject(viewName);
			var viewProxy = go.AddComponent<ViewProxy>();

			go.transform.SetParent(_viewRootTransform, false);

			if (zIndex < 0)
			{
				go.transform.SetAsFirstSibling();
			}
			else if (zIndex < _viewRootTransform.childCount)
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
			viewProxy.name = viewName;

			return viewProxy;
		}

		private IView CreateView(GameObject prefab, ViewProxy proxy, Transform parent)
		{
			Debug.Assert(prefab);
			Debug.Assert(proxy);
			Debug.Assert(parent);

			var go = Instantiate(prefab, parent, false);
			var view = go.GetComponent<IView>();

			if (view == null)
			{
				view = go.AddComponent<View>();
			}

			view.Site = proxy;
			return view;
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

		#endregion
	}
}
