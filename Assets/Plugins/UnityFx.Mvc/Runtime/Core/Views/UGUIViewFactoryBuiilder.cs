﻿// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Factory of UGUI-based <see cref="IViewFactory"/> instances.
	/// </summary>
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
		public void AddViewPrefab(string prefabPath, GameObject prefabGo)
		{
			if (prefabPath is null)
			{
				throw new ArgumentNullException(nameof(prefabPath));
			}

			if (string.IsNullOrWhiteSpace(prefabPath))
			{
				throw new ArgumentException("Invalid prefab path (name).", nameof(prefabPath));
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
		}

		/// <summary>
		/// Adds a view root.
		/// </summary>
		/// <param name="transform">A <see cref="Transform"/> to use as a view root.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="transform"/> is <see langword="null"/>.</exception>
		public void AddViewRoot(Transform transform)
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
		}

		/// <summary>
		/// Sets a delegate to use to load prefabs.
		/// </summary>
		/// <param name="loadPrefabDelegate">A delegate to load a prefab for the specified path.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="loadPrefabDelegate"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the delegate is already set.</exception>
		public void UseLoadDelegate(Func<string, Task<GameObject>> loadPrefabDelegate)
		{
			if (_loadPrefabDelegate != null)
			{
				throw new InvalidOperationException();
			}

			_loadPrefabDelegate = loadPrefabDelegate ?? throw new ArgumentNullException(nameof(loadPrefabDelegate));
		}

		/// <summary>
		/// Sets background color to use for popup views.
		/// </summary>
		/// <param name="backgroundColor">The popup background color.</param>
		public void UsePopupBackgoundColor(Color backgroundColor)
		{
			_popupBackgroundColor = backgroundColor;
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