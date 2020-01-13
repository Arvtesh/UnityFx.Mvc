// Copyright (C) 2019 Alexander Bogarsukov. All rights reserved.
// See the LICENSE.md file in the project root for more information.

using System;
using UnityEditor;
using UnityEngine;

namespace UnityFx.Mvc
{
	[CustomEditor(typeof(Presenter), true)]
	public class PresenterEditor : Editor
	{
		private Presenter _presenter;

		private void OnEnable()
		{
			_presenter = (Presenter)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUI.BeginDisabledGroup(true);

			if (_presenter.ServiceProvider is UnityEngine.Object o)
			{
				EditorGUILayout.ObjectField("Service Provider", o, typeof(UnityEngine.Object), true);
			}

			if (_presenter.ViewFactory is UnityEngine.Object o2)
			{
				EditorGUILayout.ObjectField("View Factory", o2, typeof(UnityEngine.Object), true);
			}

			var controllers = _presenter.Controllers;

			if (controllers != null && controllers.Count > 0)
			{
				var controllerId = 0;

				EditorGUILayout.LabelField("Controllers");
				EditorGUI.indentLevel += 1;

				foreach (var c in controllers)
				{
					EditorGUILayout.LabelField("#" + controllerId.ToString(), c.GetType().Name);
					controllerId++;
				}

				EditorGUI.indentLevel -= 1;
			}

			EditorGUI.EndDisabledGroup();
		}
	}
}
