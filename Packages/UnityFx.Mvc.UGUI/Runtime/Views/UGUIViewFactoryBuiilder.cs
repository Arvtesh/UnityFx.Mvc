// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Builder of UGUI-based <see cref="IViewFactory"/> instances.
	/// </summary>
	/// <seealso cref="PresenterBuilder"/>
	/// <seealso href="https://en.wikipedia.org/wiki/Builder_pattern"/>
	public sealed class UGUIViewFactoryBuilder : IViewFactoryBuilder
	{
		#region data

		private readonly GameObject _gameObject;

		private Dictionary<string, GameObject> _viewPrefabs;
		private List<Transform> _layers;
		private Func<string, Task<GameObject>> _loadPrefabDelegate;
		private Color _popupBackgroundColor = new Color(0, 0, 0, 0.5f);

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="UGUIViewFactoryBuilder"/> class.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="go"/> is <see langword="null"/>.</exception>
		public UGUIViewFactoryBuilder(GameObject go)
		{
			_gameObject = go ?? throw new ArgumentNullException(nameof(go));
		}

		#endregion

		#region IViewFactoryBuilder

		/// <summary>
		/// Adds a new view layer.
		/// </summary>
		/// <param name="transform">A <see cref="Transform"/> to use as a view root.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="transform"/> is <see langword="null"/>.</exception>
		/// <seealso cref="ViewControllerAttribute.Layer"/>
		/// <seealso cref="AddViewPrefab(string, GameObject)"/>
		/// <seealso cref="Build"/>
		public IViewFactoryBuilder AddLayer(Transform transform)
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
		public IViewFactoryBuilder AddViewPrefab(string resourceId, GameObject prefab)
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

			if (_viewPrefabs is null)
			{
				_viewPrefabs = new Dictionary<string, GameObject>();
			}

			_viewPrefabs.Add(resourceId, prefab);
			return this;
		}

		/// <summary>
		/// Sets a delegate to use to load prefabs.
		/// </summary>
		/// <param name="loadPrefabDelegate">A delegate to load a prefab for the specified path.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="loadPrefabDelegate"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the delegate is already set.</exception>
		/// <seealso cref="UsePopupBackgoundColor(Color)"/>
		/// <seealso cref="Build"/>
		public IViewFactoryBuilder UseLoadDelegate(Func<string, Task<GameObject>> loadPrefabDelegate)
		{
			if (_loadPrefabDelegate != null)
			{
				throw new InvalidOperationException();
			}

			_loadPrefabDelegate = loadPrefabDelegate ?? throw new ArgumentNullException(nameof(loadPrefabDelegate));
			return this;
		}

		/// <summary>
		/// Sets background color to use for popup views.
		/// </summary>
		/// <param name="backgroundColor">The popup background color.</param>
		/// <seealso cref="UseLoadDelegate(Func{string, Task{GameObject}})"/>
		/// <seealso cref="Build"/>
		public IViewFactoryBuilder UsePopupBackgoundColor(Color backgroundColor)
		{
			_popupBackgroundColor = backgroundColor;
			return this;
		}

		/// <summary>
		/// Creates a <see cref="MonoBehaviour"/>-based implementation of <see cref="IViewService"/>.
		/// </summary>
		public IViewService Build()
		{
			var viewFactory = _gameObject.AddComponent<UGUIViewFactory>();

			viewFactory.SetPopupBackgrounColor(_popupBackgroundColor);
			viewFactory.SetLoadPrefabDelegate(_loadPrefabDelegate);
			viewFactory.SetViewPrefabs(_viewPrefabs);
			viewFactory.SetViewRoots(_layers);

			return viewFactory;
		}

		#endregion
	}
}
