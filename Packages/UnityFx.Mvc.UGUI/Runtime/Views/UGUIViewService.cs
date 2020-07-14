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
	/// <seealso cref="UGUIViewServiceBuilder"/>
	internal sealed partial class UGUIViewService : ViewService
	{
		#region data
		#endregion

		#region interface

		internal void OnDestroyView(UGUIViewProxy view)
		{
			if (view)
			{
				UpdateViewStack(view.transform.parent);
			}
		}

		#endregion

		#region ViewService

		protected override IReadOnlyCollection<IView> CreateViewCollection()
		{
			return new ViewCollection(this);
		}

		#endregion

		#region IViewFactory

		public async override Task<IView> CreateViewAsync(string resourceId, int layer, int zIndex, ViewControllerFlags flags, Transform parent)
		{
			ThrowIfDisposed();

			UGUIViewProxy viewProxy = null;

			try
			{
				var exclusive = (flags & ViewControllerFlags.Exclusive) != 0;
				var modal = (flags & ViewControllerFlags.Modal) != 0;
				var viewRoot = Layers[layer];

				viewProxy = CreateViewProxy(viewRoot, resourceId, zIndex, exclusive, modal);

				var viewPrefab = await PrefabRepository.LoadPrefabAsync(resourceId);
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

		public override void DestroyView(IView view)
		{
			// TODO: Unload view prefab if there are no views instantieted from it.
			view?.Dispose();
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
				view = MvcUtilities.InstantiateViewPrefab<UGUIView>(prefab, parent ?? viewProxy.transform);
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

				image.color = PopupBackgroundColor;
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

		#endregion
	}
}
