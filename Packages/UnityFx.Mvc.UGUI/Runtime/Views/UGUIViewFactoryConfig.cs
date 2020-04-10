// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Configuration of UGUI-based <see cref="IViewFactory"/> implementations.
	/// </summary>
	/// <seealso cref="UGUIViewFactoryBuilder"/>
	[CreateAssetMenu(fileName = "UiConfig", menuName = "UnityFx/Mvc/UGUI Config")]
	public sealed class UGUIViewFactoryConfig : ScriptableObject
	{
		#region data

#pragma warning disable 0649

		[SerializeField]
		private Color _popupBackgroundColor = new Color(0, 0, 0, 0.5f);
		[SerializeField, HideInInspector]
		private List<PrefabDesc> _prefabs;

#if UNITY_EDITOR

		[SerializeField, HideInInspector]
		private List<PrefabGroup> _prefabGroups;
		[SerializeField, HideInInspector]
		private List<PrefabDesc> _customPrefabs;

#endif

#pragma warning restore 0649

		#endregion

		#region interface

#if UNITY_EDITOR

		[Serializable]
		internal struct PrefabGroup
		{
			public string PathPrefix;
			public string Folder;
		}

		internal IReadOnlyCollection<PrefabGroup> PrefabGroups => _prefabGroups;

		internal IReadOnlyList<PrefabDesc> CustomPrefabs => _customPrefabs;

		internal void AddPrefab(PrefabDesc prefab)
		{
			_prefabs.Add(prefab);
		}

		internal bool AddPrefab(string pathPrefix, GameObject prefab)
		{
			if (prefab)
			{
				var path = prefab.name;

				if (string.IsNullOrWhiteSpace(path))
				{
					path = prefab.GetInstanceID().ToString();
				}

				if (!string.IsNullOrWhiteSpace(pathPrefix))
				{
					path = pathPrefix + '/' + path;
				}

				_prefabs.Add(new PrefabDesc()
				{
					Path = path,
					Prefab = prefab
				});

				return true;
			}

			return false;
		}

		internal void ClearPrefabs()
		{
			_prefabs.Clear();
		}

#endif

		[Serializable]
		public struct PrefabDesc
		{
			public string Path;
			public GameObject Prefab;
		}

		public Color PopupBackgroundColor => _popupBackgroundColor;

		public IReadOnlyList<PrefabDesc> Prefabs => _prefabs;

		#endregion
	}
}
