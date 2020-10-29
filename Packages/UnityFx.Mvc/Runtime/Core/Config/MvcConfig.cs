// Copyright (C) 2020 Program-Ace, LLC. All rights reserved.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Configuration of MVC application.
	/// </summary>
	[CreateAssetMenu(fileName = "MvcConfig", menuName = "UnityFx/Mvc/MVC Config")]
	public class MvcConfig : ScriptableObject, IPrefabRepository, IViewControllerBindings
	{
		#region data

#pragma warning disable 0649

		[SerializeField]
		private Color _popupBackgroundColor = new Color(0, 0, 0, 0.5f);
		[SerializeField, HideInInspector]
		private List<ViewControllerInfo> _viewControllers = new List<ViewControllerInfo>();

#if UNITY_EDITOR

		[SerializeField, HideInInspector]
		private string _defaultNamespace;
		[SerializeField, HideInInspector]
		private string _baseControllerTypePath;
		[SerializeField, HideInInspector]
		private string _baseViewTypePath;
		[SerializeField, HideInInspector]
		private List<string> _folders;

#endif

#pragma warning restore 0649

		private Dictionary<object, GameObject> _preloadedPrefabs = new Dictionary<object, GameObject>();
		private Dictionary<object, GameObject> _prefabs = new Dictionary<object, GameObject>();

		private Func<object, Task<GameObject>> _loadPrefabDelegate;
		private Dictionary<object, Task<GameObject>> _loadTasks;

		private bool _disposed;

		#endregion

		#region interface

#if UNITY_EDITOR

		internal const string DefaultControllerPath = "Packages/com.unityfx.mvc/Runtime/Core/Mvc/ViewController.cs";
		internal const string DefaultViewPath = "Packages/com.unityfx.mvc/Runtime/Core/Mvc/View.cs";

		internal IReadOnlyList<string> SearchFolders => _folders;

		internal List<ViewControllerInfo> ViewControllers => _viewControllers;

		internal string BaseControllerPath => _baseControllerTypePath;

		internal string BaseViewPath => _baseViewTypePath;

		internal static string GetResourceId(Type controllerType)
		{
			return PresentUtilities.GetControllerName(controllerType);
		}

		internal static string GetResourceId(string controllerName)
		{
			return PresentUtilities.GetControllerName(controllerName);
		}

		internal void AddItem(ViewControllerInfo item)
		{
			_viewControllers.Add(item);
		}

		internal void Clear()
		{
			_viewControllers.Clear();
		}

#endif

		[Serializable]
		internal struct ViewControllerInfo
		{
#if UNITY_EDITOR
			public string ControllerScriptPath;
			public string ViewScriptPath;
#endif
			public string ViewResourceId;
			public GameObject ViewPrefab;

		}

		/// <summary>
		/// Gets background color of popup windows.
		/// </summary>
		public Color PopupBackgroundColor => _popupBackgroundColor;

		/// <summary>
		/// Gets or sets a delegate used for loading view prefabs.
		/// </summary>
		public Func<object, Task<GameObject>> LoadPrefabDelegate { get => _loadPrefabDelegate; set => _loadPrefabDelegate = value; }

		#endregion

		#region ScriptableObject

		private void OnEnable()
		{
			if (_viewControllers != null)
			{
				foreach (var item in _viewControllers)
				{
					if (item.ViewPrefab && !string.IsNullOrWhiteSpace(item.ViewResourceId))
					{
						_preloadedPrefabs[item.ViewResourceId] = item.ViewPrefab;
					}
				}
			}

#if !UNITY_EDITOR

			_viewControllers = null;

#endif
		}

		private void OnDestroy()
		{
			_disposed = true;
		}

		#endregion

		#region IPrefabRepository

		/// <inheritdoc/>
		public async Task<GameObject> LoadPrefabAsync(object key)
		{
			ThrowIfDisposed();

			if (key is null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			if (_prefabs.TryGetValue(key, out var prefab))
			{
				return prefab;
			}

			if (_preloadedPrefabs.TryGetValue(key, out prefab))
			{
				return prefab;
			}

			if (_loadPrefabDelegate != null)
			{
				if (_loadTasks is null)
				{
					_loadTasks = new Dictionary<object, Task<GameObject>>();
				}
				
				if (_loadTasks.TryGetValue(key, out var task))
				{
					prefab = await task;
					TryAddPrefab(key, prefab);
					return prefab;
				}
				else
				{
					task = _loadPrefabDelegate(key);

					_loadTasks.Add(key, task);

					try
					{
						prefab = await task;
						TryAddPrefab(key, prefab);
						return prefab;
					}
					finally
					{
						_loadTasks.Remove(key);
					}
				}
			}

			throw new KeyNotFoundException();
		}

		/// <inheritdoc/>
		public void ReleasePrefab(GameObject prefab)
		{
			if (prefab)
			{
				foreach (var kvp in _prefabs)
				{
					if (kvp.Value == prefab)
					{
						if (_prefabs.Remove(kvp.Key))
						{
							GameObject.Destroy(prefab);
						}

						break;
					}
				}
			}
		}

		#endregion

		#region IViewControllerBindings

		/// <inheritdoc/>
		public object GetViewKey(Type controllerType)
		{
			if (controllerType is null)
			{
				return null;
			}

			return PresentUtilities.GetControllerName(controllerType);
		}

		#endregion

		#region implementation

		private void TryAddPrefab(object key, GameObject prefab)
		{
			if (!_disposed)
			{
				if (_prefabs.TryGetValue(key, out var existingPrefab))
				{
					if (existingPrefab != prefab)
					{
						Debug.LogWarning($"A prefab '{key}' was manually added while loading operation was pending. The existing prefab '{existingPrefab.name}' will be replaced with the loaded one '{prefab.name}'.");
						_prefabs[key] = prefab;
					}
				}
				else
				{
					_prefabs.Add(key, prefab);
				}
			}
		}

		private void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		#endregion
	}
}
