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
	public class MvcConfig : ScriptableObject, IViewControllerBindings
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

		private Dictionary<string, GameObject> _prefabsMap = new Dictionary<string, GameObject>();

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
			return MvcUtilities.GetControllerName(controllerType);
		}

		internal static string GetResourceId(string controllerName)
		{
			return MvcUtilities.GetControllerName(controllerName);
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
		/// Gets a collectino of pre-loaded view prefabs.
		/// </summary>
		public IReadOnlyDictionary<string, GameObject> Prefabs => _prefabsMap;

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
						_prefabsMap[item.ViewResourceId] = item.ViewPrefab;
					}
				}
			}

#if !UNITY_EDITOR

			_viewControllers = null;

#endif
		}

		#endregion

		#region IViewControllerBindings

		/// <inheritdoc/>
		public string GetViewResourceId(Type controllerType)
		{
			if (controllerType is null)
			{
				throw new ArgumentNullException(nameof(controllerType));
			}

			return MvcUtilities.GetControllerName(controllerType);
		}

		#endregion
	}
}
