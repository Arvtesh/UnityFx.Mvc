// Copyright (C) 2019 Alexander Bogarsukov. All rights reserved.
// See the LICENSE.md file in the project root for more information.

using System;
using UnityEditor;
using UnityEngine;

namespace UnityFx.Mvc
{
	[CustomEditor(typeof(PresenterBehaviour), true)]
	public class PresenterEditor : Editor
	{
		private Presenter _presenter;

		private void OnEnable()
		{
			_presenter = ((PresenterBehaviour)target).Presenter;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUI.BeginDisabledGroup(true);

			if (_presenter.ServiceProvider is UnityEngine.Object o)
			{
				EditorGUILayout.ObjectField("Service Provider", o, typeof(UnityEngine.Object), true);
			}

			var controllers = _presenter.Controllers;

			if (controllers != null && controllers.Count > 0)
			{
				var controllerId = 0;

				EditorGUILayout.LabelField("Controllers");
				EditorGUI.indentLevel += 1;

				foreach (var c in controllers)
				{
					if (c is null)
					{
						EditorGUILayout.LabelField("#" + controllerId.ToString(), "null");
					}
					else
					{
						EditorGUILayout.LabelField("#" + controllerId.ToString(), c.GetType().Name);
					}

					controllerId++;
				}

				EditorGUI.indentLevel -= 1;
			}

			EditorGUI.EndDisabledGroup();
		}
	}
}
