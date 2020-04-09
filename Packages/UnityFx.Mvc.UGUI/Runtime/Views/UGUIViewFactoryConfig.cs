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
	[CreateAssetMenu(fileName = "MvcConfig", menuName = "UnityFx/Mvc/MVC Config (UGUI)")]
	public sealed class UGUIViewFactoryConfig : ScriptableObject
	{
		#region data

#pragma warning disable 0649

		[SerializeField]
		private Color _popupBackgroundColor = new Color(0, 0, 0, 0.5f);
		[SerializeField, HideInInspector]
		private List<PrefabGroup> _prefabGroups;
		[SerializeField, HideInInspector]
		private List<PrefabDesc> _prefabs;

#pragma warning restore 0649

		#endregion

		#region interface

		[Serializable]
		public struct PrefabGroup
		{
			public string PathPrefix;
			public string Folder;
		}

		[Serializable]
		public struct PrefabDesc
		{
			public string Path;
			public GameObject Prefab;
		}

		public Color PopupBackgroundColor { get => _popupBackgroundColor; set => _popupBackgroundColor = value; }

		public IList<PrefabGroup> PrefabGroups => _prefabGroups;

		public IList<PrefabDesc> Prefabs => _prefabs;

		#endregion
	}
}
