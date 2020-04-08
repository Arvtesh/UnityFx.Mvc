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
		[SerializeField]
		private string _prefabPathPrefix;
		[SerializeField]
		private List<GameObject> _viewPrefabs;

#pragma warning restore 0649

		#endregion

		#region interface

		public Color PopupBackgroundColor => _popupBackgroundColor;

		public string PrefabPathPrefix => _prefabPathPrefix;

		public IList<GameObject> ViewPrefabs => _viewPrefabs;

		#endregion
	}
}
