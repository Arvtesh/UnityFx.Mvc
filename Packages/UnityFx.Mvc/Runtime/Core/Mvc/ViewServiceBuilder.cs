// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Builder of <see cref="ViewService"/> instances.
	/// </summary>
	/// <seealso cref="ViewService"/>
	/// <seealso href="https://en.wikipedia.org/wiki/Builder_pattern"/>
	public abstract class ViewServiceBuilder
	{
		#region data

		private readonly GameObject _gameObject;
		private readonly IPrefabRepository _prefabRepository;

		private List<Transform> _layers;
		private Color _popupBackgroundColor = new Color(0, 0, 0, 0.5f);

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewServiceBuilder"/> class.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="go"/> is <see langword="null"/>.</exception>
		protected ViewServiceBuilder(GameObject go)
		{
			_gameObject = go ?? throw new ArgumentNullException(nameof(go));
			_prefabRepository = new SimplePrefabRepository();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewServiceBuilder"/> class.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="go"/> is <see langword="null"/>.</exception>
		protected ViewServiceBuilder(GameObject go, IPrefabRepository prefabRepository)
		{
			_gameObject = go ?? throw new ArgumentNullException(nameof(go));
			_prefabRepository = prefabRepository ?? throw new ArgumentNullException(nameof(prefabRepository));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewServiceBuilder"/> class.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="go"/> is <see langword="null"/>.</exception>
		protected ViewServiceBuilder(GameObject go, MvcConfig config)
		{
			_gameObject = go ?? throw new ArgumentNullException(nameof(go));
			_prefabRepository = config ?? throw new ArgumentNullException(nameof(config));
			_popupBackgroundColor = config.PopupBackgroundColor;
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <seealso cref="Build"/>
		protected abstract ViewService Build(GameObject go);

		/// <summary>
		/// Adds a new view layer.
		/// </summary>
		/// <param name="transform">A <see cref="Transform"/> to use as a view root.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="transform"/> is <see langword="null"/>.</exception>
		/// <seealso cref="ViewControllerAttribute.Layer"/>
		/// <seealso cref="AddViewPrefab(string, GameObject)"/>
		/// <seealso cref="Build"/>
		public ViewServiceBuilder AddLayer(Transform transform)
		{
			if (transform is null)
			{
				throw new ArgumentNullException(nameof(transform));
			}

			if (_layers is null)
			{
				_layers = new List<Transform>();
			}

			_layers.Add(transform);
			return this;
		}

		/// <summary>
		/// Adds a preloaded view prefab.
		/// </summary>
		/// <param name="resourceId">A path to assosiate the <paramref name="prefab"/> with.</param>
		/// <param name="prefab">The preloaded prefab.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="resourceId"/> or <paramref name="prefab"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown is <paramref name="resourceId"/> is invalid.</exception>
		/// <seealso cref="AddLayer(Transform)"/>
		/// <seealso cref="Build"/>
		public ViewServiceBuilder AddViewPrefab(string resourceId, GameObject prefab)
		{
			if (resourceId is null)
			{
				throw new ArgumentNullException(nameof(resourceId));
			}

			if (string.IsNullOrWhiteSpace(resourceId))
			{
				throw new ArgumentException(Messages.Format_InvalidPrefabPath(), nameof(resourceId));
			}

			if (prefab is null)
			{
				throw new ArgumentNullException(nameof(prefab));
			}

			_prefabRepository.PrefabCache.Add(resourceId, prefab);
			return this;
		}

		/// <summary>
		/// Sets background color to use for popup views.
		/// </summary>
		/// <param name="backgroundColor">The popup background color.</param>
		/// <seealso cref="UseLoadDelegate(Func{string, Task{GameObject}})"/>
		/// <seealso cref="Build"/>
		public ViewServiceBuilder UsePopupBackgoundColor(Color backgroundColor)
		{
			_popupBackgroundColor = backgroundColor;
			return this;
		}

		/// <summary>
		/// Creates a <see cref="MonoBehaviour"/>-based implementation of <see cref="IViewService"/>.
		/// </summary>
		public IViewService Build()
		{
			var viewFactory = Build(_gameObject);

			viewFactory.SetPrefabRepository(_prefabRepository);
			viewFactory.SetPopupBackgrounColor(_popupBackgroundColor);
			viewFactory.SetLayers(_layers);

			return viewFactory;
		}

		#endregion
	}
}
