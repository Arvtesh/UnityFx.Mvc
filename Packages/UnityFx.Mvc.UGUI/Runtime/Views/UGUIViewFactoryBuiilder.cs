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
	public sealed class UGUIViewFactoryBuilder
	{
		#region data

		private readonly GameObject _gameObject;

		private Dictionary<string, GameObject> _viewPrefabs;
		private List<Transform> _viewRoots;
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

		/// <summary>
		/// Adds a preloaded view prefab.
		/// </summary>
		/// <param name="prefabPath">A path to assosiate the <paramref name="prefabGo"/> with.</param>
		/// <param name="prefabGo">The preloaded prefab.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="prefabPath"/> or <paramref name="prefabGo"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown is <paramref name="prefabPath"/> is invalid.</exception>
		/// <seealso cref="AddLayer(Transform)"/>
		/// <seealso cref="AddViewPrefabs(GameObject[])"/>
		/// <seealso cref="AddViewPrefabs(string, GameObject[])"/>
		/// <seealso cref="Build"/>
		public UGUIViewFactoryBuilder AddViewPrefab(string prefabPath, GameObject prefabGo)
		{
			if (prefabPath is null)
			{
				throw new ArgumentNullException(nameof(prefabPath));
			}

			if (string.IsNullOrWhiteSpace(prefabPath))
			{
				throw new ArgumentException(Messages.Format_InvalidPrefabPath(), nameof(prefabPath));
			}

			if (prefabGo is null)
			{
				throw new ArgumentNullException(nameof(prefabGo));
			}

			if (_viewPrefabs is null)
			{
				_viewPrefabs = new Dictionary<string, GameObject>();
			}

			_viewPrefabs.Add(prefabPath, prefabGo);
			return this;
		}

		/// <summary>
		/// Adds preloaded view prefabs.
		/// </summary>
		/// <param name="prefabs">The preloaded prefabs.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="prefabPath"/> or <paramref name="prefabGo"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if any of the prefabs is <see langword="null"/>.</exception>
		/// <seealso cref="AddLayer(Transform)"/>
		/// <seealso cref="AddViewPrefab(string, GameObject)"/>
		/// <seealso cref="AddViewPrefabs(string, GameObject[])"/>
		/// <seealso cref="AddViewPrefabs(string, IEnumerable{GameObject})"/>
		/// <seealso cref="Build"/>
		public UGUIViewFactoryBuilder AddViewPrefabs(params GameObject[] prefabs)
		{
			return AddViewPrefabs(null, (IEnumerable<GameObject>)prefabs);
		}

		/// <summary>
		/// Adds preloaded view prefabs.
		/// </summary>
		/// <param name="pathPrefix">A prefix string to add to the prefab names.</param>
		/// <param name="prefabs">The preloaded prefabs.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="prefabPath"/> or <paramref name="prefabGo"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if any of the prefabs is <see langword="null"/>.</exception>
		/// <seealso cref="AddLayer(Transform)"/>
		/// <seealso cref="AddViewPrefab(string, GameObject)"/>
		/// <seealso cref="AddViewPrefabs(GameObject[])"/>
		/// <seealso cref="AddViewPrefabs(string, IEnumerable{GameObject})"/>
		/// <seealso cref="Build"/>
		public UGUIViewFactoryBuilder AddViewPrefabs(string pathPrefix, params GameObject[] prefabs)
		{
			return AddViewPrefabs(pathPrefix, (IEnumerable<GameObject>)prefabs);
		}

		/// <summary>
		/// Adds preloaded view prefabs.
		/// </summary>
		/// <param name="pathPrefix">A prefix string to add to the prefab names.</param>
		/// <param name="prefabs">The preloaded prefabs.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="prefabPath"/> or <paramref name="prefabGo"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if any of the prefabs is <see langword="null"/>.</exception>
		/// <seealso cref="AddLayer(Transform)"/>
		/// <seealso cref="AddViewPrefab(string, GameObject)"/>
		/// <seealso cref="AddViewPrefabs(GameObject[])"/>
		/// <seealso cref="Build"/>
		public UGUIViewFactoryBuilder AddViewPrefabs(string pathPrefix, IEnumerable<GameObject> prefabs)
		{
			if (prefabs is null)
			{
				throw new ArgumentNullException(nameof(prefabs));
			}

			if (_viewPrefabs is null)
			{
				_viewPrefabs = new Dictionary<string, GameObject>();
			}

			if (string.IsNullOrEmpty(pathPrefix))
			{
				foreach (var go in prefabs)
				{
					if (go)
					{
						_viewPrefabs.Add(go.name, go);
					}
					else
					{
						throw new ArgumentException(Messages.Format_PrefabIsNull(), nameof(prefabs));
					}
				}
			}
			else
			{
				foreach (var go in prefabs)
				{
					if (go)
					{
						_viewPrefabs.Add(pathPrefix + go.name, go);
					}
					else
					{
						throw new ArgumentException(Messages.Format_PrefabIsNull(), nameof(prefabs));
					}
				}
			}

			return this;
		}

		/// <summary>
		/// Adds a new view layer.
		/// </summary>
		/// <param name="transform">A <see cref="Transform"/> to use as a view root.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="transform"/> is <see langword="null"/>.</exception>
		/// <seealso cref="ViewControllerAttribute.Layer"/>
		/// <seealso cref="AddViewPrefab(string, GameObject)"/>
		/// <seealso cref="Build"/>
		public UGUIViewFactoryBuilder AddLayer(Transform transform)
		{
			if (transform is null)
			{
				throw new ArgumentNullException(nameof(transform));
			}

			if (_viewRoots is null)
			{
				_viewRoots = new List<Transform>();
			}

			_viewRoots.Add(transform);
			return this;
		}

		/// <summary>
		/// Applies the specififed configuration.
		/// </summary>
		/// <param name="config">The configuration asset.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="config"/> is <see langword="null"/>.</exception>
		/// <seealso cref="UsePopupBackgoundColor(Color)"/>
		/// <seealso cref="AddViewPrefabs(string, IEnumerable{GameObject})"/>
		/// <seealso cref="Build"/>
		public UGUIViewFactoryBuilder UseConfig(UGUIViewFactoryConfig config)
		{
			if (config is null)
			{
				throw new ArgumentNullException(nameof(config));
			}

			_popupBackgroundColor = config.PopupBackgroundColor;

			foreach (var item in config.Prefabs)
			{
				AddViewPrefab(item.Path, item.Prefab);
			}

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
		public UGUIViewFactoryBuilder UseLoadDelegate(Func<string, Task<GameObject>> loadPrefabDelegate)
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
		public UGUIViewFactoryBuilder UsePopupBackgoundColor(Color backgroundColor)
		{
			_popupBackgroundColor = backgroundColor;
			return this;
		}

		/// <summary>
		/// Creates a <see cref="MonoBehaviour"/>-based implementation of <see cref="IViewFactory"/>.
		/// </summary>
		public IViewFactory Build()
		{
			var viewFactory = _gameObject.AddComponent<UGUIViewFactory>();

			viewFactory.SetPopupBackgrounColor(_popupBackgroundColor);
			viewFactory.SetLoadPrefabDelegate(_loadPrefabDelegate);
			viewFactory.SetViewPrefabs(_viewPrefabs);
			viewFactory.SetViewRoots(_viewRoots);

			return viewFactory;
		}

		#endregion
	}
}
