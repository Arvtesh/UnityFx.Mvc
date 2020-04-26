// Copyright (C) 2019 Alexander Bogarsukov. All rights reserved.
// See the LICENSE.md file in the project root for more information.

using System;
using UnityEditor;
using UnityEngine;

namespace UnityFx.Mvc
{
	[CustomEditor(typeof(ViewService), true)]
	public class ViewFactoryEditor : Editor
	{
		private ViewService _viewFactory;

		private void OnEnable()
		{
			_viewFactory = (ViewService)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.ColorField("Popup Background", _viewFactory.PopupBackgroundColor);

			var viewRoots = _viewFactory.Layers;

			if (viewRoots != null && viewRoots.Count > 0)
			{
				EditorGUILayout.LabelField("View Roots");
				EditorGUI.indentLevel += 1;

				for (var i = 0; i < viewRoots.Count; i++)
				{
					EditorGUILayout.ObjectField("#" + i.ToString(), viewRoots[i], typeof(Transform), true);
				}

				EditorGUI.indentLevel -= 1;
			}

			var prefabs = _viewFactory.Prefabs;

			if (prefabs != null && prefabs.Count > 0)
			{
				var prefabId = 0;

				EditorGUILayout.LabelField("View Prefabs");
				EditorGUI.indentLevel += 1;

				foreach (var prefab in prefabs)
				{
					EditorGUILayout.LabelField("#" + prefabId.ToString(), prefab.Key);
					prefabId++;
				}

				EditorGUI.indentLevel -= 1;
			}

			var views = _viewFactory.Views;

			if (views != null && views.Count > 0)
			{
				var viewId = 0;

				EditorGUILayout.LabelField("Views");
				EditorGUI.indentLevel += 1;

				foreach (var view in views)
				{
					EditorGUILayout.ObjectField("#" + viewId.ToString(), view as View, typeof(View), true);
					viewId++;
				}

				EditorGUI.indentLevel -= 1;
			}

			EditorGUI.EndDisabledGroup();
		}
	}
}
