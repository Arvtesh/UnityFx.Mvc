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

		private IPrefabRepository _prefabRepository;
		private IReadOnlyList<Transform> _layers;
		private IReadOnlyCollection<IView> _views;
		private Color _popupBgColor;
		private bool _disposed;

		#endregion

		#region interface

		internal void SetPopupBackgrounColor(Color color)
		{
			_popupBgColor = color;
		}

		internal void SetPrefabRepository(IPrefabRepository prefabRepository)
		{
			_prefabRepository = prefabRepository;
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
		/// Gets the prefab repository.
		/// </summary>
		protected internal IPrefabRepository PrefabRepository => _prefabRepository;

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

		public abstract Task<IView> CreateViewAsync(string resourceId, Transform parent);

		public abstract void ReleaseView(IView view);

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
