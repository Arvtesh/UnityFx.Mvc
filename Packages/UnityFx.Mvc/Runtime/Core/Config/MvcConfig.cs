// Copyright (c) 2018-2020 Alexander Bogarsukov.
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
	public sealed class MvcConfig : ScriptableObject
	{
		#region data

#pragma warning disable 0649

		[SerializeField, HideInInspector]
		private List<ViewControllerInfo> _viewControllers;

#if UNITY_EDITOR

		// TODO

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

		// TODO

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
		public class ViewControllerInfo
		{
#if UNITY_EDITOR
			public string ControllerScriptPath;
			public string ViewScriptPath;
#endif
			public string ViewPath;
			public GameObject ViewPrefab;
		}

		public IReadOnlyList<ViewControllerInfo> ViewControllers => _viewControllers;

		#endregion
	}
}
