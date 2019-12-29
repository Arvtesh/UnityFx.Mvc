// Copyright (C) 2019 Alexander Bogarsukov. All rights reserved.
// See the LICENSE.md file in the project root for more information.

using System;
using UnityEditor;
using UnityEngine;

namespace UnityFx.Mvc
{
	[CustomEditor(typeof(UGUIViewFactory))]
	public class ViewFactoryEditor : Editor
	{
		private UGUIViewFactory _viewFactory;

		private void OnEnable()
		{
			_viewFactory = (UGUIViewFactory)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var views = _viewFactory.Views;

			if (views != null && views.Count > 0)
			{
				var viewId = 0;

				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.LabelField("Views");
				EditorGUI.indentLevel += 1;

				foreach (var view in _viewFactory.Views)
				{
					EditorGUILayout.ObjectField("#" + viewId.ToString(), view as View, typeof(View), true);
					viewId++;
				}

				EditorGUI.indentLevel -= 1;
				EditorGUI.EndDisabledGroup();
			}
		}
	}
}
