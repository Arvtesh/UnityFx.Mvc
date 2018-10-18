// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Prefab view manager.
	/// </summary>
	public abstract class ViewManagerBehaviour<TView> : DisposableBehaviour, IViewFactory, IServiceProvider where TView : class, IView
	{
		#region data

		private IViewLoader _viewLoader;

		#endregion

		#region interface
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

		#region IServiceProvider

		/// <summary>
		/// Gets the service object of the specified type.
		/// </summary>
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(IViewFactory))
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

		#endregion
	}
}
