// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Prefab view manager.
	/// </summary>
	public abstract class ViewManagerBehaviour<TView> : DisposableBehaviour, IContainer, IViewFactory, IServiceProvider where TView : ViewBehaviour
	{
		#region data

		private IViewLoader _viewLoader;
		private ComponentCollection _components;

		#endregion

		#region interface

		/// <summary>
		/// Adds the specified view to the <see cref="IContainer"/> at the end of the list.
		/// </summary>
		public void Add(TView view, int index)
		{
			if (view == null)
			{
				throw new ArgumentNullException(nameof(view));
			}

			if (index < 0 || index > transform.childCount)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			AddViewInternal(view, null, index);
		}

		/// <summary>
		/// Updates a view at the specific index. This is called each time a view is added or removed. Default implementation
		/// updates view position.
		/// </summary>
		/// <param name="viewTransform">Transform of the view to update.</param>
		/// <param name="index">Index of the view.</param>
		protected virtual void UpdateView(Transform viewTransform, int index)
		{
			viewTransform.localPosition = new Vector3(0, 0, 10 + 1 * index);
		}

		#endregion

		#region MonoBehaviour

		private void Awake()
		{
			_viewLoader = GetComponent<IViewLoader>();
		}

		#endregion

		#region IViewFactory

		/// <summary>
		/// Asynchronously loads the specified view.
		/// </summary>
		public IAsyncOperation<IView> LoadViewAsync(string viewId, IView insertAfter)
		{
			if (viewId == null)
			{
				throw new ArgumentNullException(nameof(viewId));
			}

			var insertAfterBehaviour = insertAfter as ViewBehaviour;

			if (insertAfterBehaviour)
			{
				var insertAfterSite = insertAfterBehaviour.transform.parent;

				if (insertAfterSite && insertAfterSite.parent == transform)
				{
					return LoadViewInternal(viewId, insertAfterSite.GetSiblingIndex() + 1);
				}
				else
				{
					throw new InvalidOperationException();
				}
			}
			else
			{
				return LoadViewInternal(viewId, 0);
			}
		}

		#endregion

		#region IContainer

		/// <summary>
		/// Gets all the components in the <see cref="IContainer"/>.
		/// </summary>
		public ComponentCollection Components
		{
			get
			{
				if (_components == null)
				{
					// TODO
				}

				return _components;
			}
		}

		/// <summary>
		/// Adds the specified component to the <see cref="IContainer"/> at the end of the list.
		/// Only components of type <typeparamref name="TView"/> can be added.
		/// </summary>
		/// <param name="component">The component to add.</param>
		/// <seealso cref="Add(IComponent, string)"/>
		/// <seealso cref="Remove(IComponent)"/>
		public void Add(IComponent component)
		{
			Add(component, null);
		}

		/// <summary>
		/// Adds the specified component to the <see cref="IContainer"/> at the end of the list, and assigns a name to the component.
		/// Only components of type <typeparamref name="TView"/> can be added.
		/// </summary>
		/// <param name="component">The component to add.</param>
		/// <param name="name">The unique, case-insensitive name to assign to the component.-or- null that leaves the component unnamed.</param>
		/// <seealso cref="Add(IComponent)"/>
		/// <seealso cref="Remove(IComponent)"/>
		public void Add(IComponent component, string name)
		{
			if (component is TView)
			{
				AddViewInternal(component as TView, name, transform.childCount);
			}
		}

		/// <summary>
		/// Removes a component from the <see cref="IContainer"/>.
		/// </summary>
		/// <param name="component">The component to remove.</param>
		/// <seealso cref="Add(IComponent)"/>
		/// <seealso cref="Add(IComponent, string)"/>
		public void Remove(IComponent component)
		{
			if (component is TView)
			{
				for (var i = 0; i < transform.childCount; ++i)
				{
					var siteTransform = transform.GetChild(i);
					var site = siteTransform.GetComponent<ISite>();

					if (site != null && site.Component == component)
					{
						Destroy(siteTransform.gameObject);
						UpdateViews(i);
						_components = null;
					}
				}
			}
		}

		#endregion

		#region IServiceProvider

		/// <summary>
		/// Gets the service object of the specified type.
		/// </summary>
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(IViewFactory) || serviceType == typeof(IContainer))
			{
				return this;
			}
			else if (serviceType == typeof(IViewLoader))
			{
				return _viewLoader;
			}

			return null;
		}

		#endregion

		#region implementation

		private void AddViewInternal(TView view, string name, int index)
		{
			var site = view.Site;

			if (site != null)
			{
				var container = site.Container;

				if (ReferenceEquals(container, this))
				{
					return;
				}

				container.Remove(view);
			}

			var go = new GameObject(name ?? view.name);
			go.transform.SetParent(transform, false);
			go.transform.SetSiblingIndex(index);

			view.transform.SetParent(go.transform, false);
			view.Site = go.AddComponent<SiteBehaviour>();

			UpdateViews(index);
			_components = null;
		}

		private IAsyncOperation<IView> LoadViewInternal(string viewId, int index)
		{
			var go = new GameObject(viewId);
			go.transform.SetParent(transform, false);
			go.transform.SetSiblingIndex(index);

			var op = _viewLoader.LoadViewAsync(viewId, go.transform);

			if (op.IsCompleted)
			{
				LoadViewCompleted(go, op);
			}
			else
			{
				op.Completed += (sender, e) =>
				{
					LoadViewCompleted(go, op);
				};
			}

			return op;
		}

		private void LoadViewCompleted(GameObject siteGo, IAsyncOperation<IView> op)
		{
			if (op.IsCompletedSuccessfully)
			{
				var view = op.Result;

				if (view is TView)
				{
					op.Result.Site = siteGo.AddComponent<SiteBehaviour>();
					UpdateViews(siteGo.transform.GetSiblingIndex());
				}
				else
				{
					Debug.LogError(string.Format("Invalid view type: {0}; {1} is expected.", view.GetType(), typeof(TView)), this);
					Destroy(siteGo);
				}
			}
			else
			{
				Debug.LogException(op.Exception, this);
				Destroy(siteGo);
			}
		}

		private void UpdateViews(int startIndex)
		{
			for (var i = startIndex; i < transform.childCount; ++i)
			{
				var siteTransform = transform.GetChild(i);
				UpdateView(siteTransform, i);
			}
		}

		#endregion
	}
}
