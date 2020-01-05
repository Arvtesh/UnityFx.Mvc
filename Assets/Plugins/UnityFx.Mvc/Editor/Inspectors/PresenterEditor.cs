// Copyright (C) 2019 Alexander Bogarsukov. All rights reserved.
// See the LICENSE.md file in the project root for more information.

using System;
using UnityEditor;
using UnityEngine;

namespace UnityFx.Mvc
{
	[CustomEditor(typeof(PresenterBase), true)]
	public class PresenterEditor : Editor
	{
		private PresenterBase _presenter;

		private void OnEnable()
		{
			_presenter = (PresenterBase)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var controllers = _presenter.GetControllers();

			if (controllers != null && controllers.Count > 0)
			{
				var controllerId = 0;

				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.LabelField("Controllers");
				EditorGUI.indentLevel += 1;

				foreach (var c in controllers)
				{
					EditorGUILayout.LabelField("#" + controllerId.ToString(), c.GetType().Name);
					controllerId++;
				}

				EditorGUI.indentLevel -= 1;
				EditorGUI.EndDisabledGroup();
			}
		}
	}
}
