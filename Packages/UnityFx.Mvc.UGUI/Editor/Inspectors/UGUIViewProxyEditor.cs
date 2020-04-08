// Copyright (C) 2019 Alexander Bogarsukov. All rights reserved.
// See the LICENSE.md file in the project root for more information.

using System;
using UnityEditor;
using UnityEngine;

namespace UnityFx.Mvc
{
	[CustomEditor(typeof(UGUIViewProxy), true)]
	public class UGUIViewProxyEditor : Editor
	{
		private UGUIViewProxy _viewProxy;

		private void OnEnable()
		{
			_viewProxy = (UGUIViewProxy)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUI.BeginDisabledGroup(true);

			EditorGUILayout.ObjectField("Factory", _viewProxy.Factory, typeof(MonoBehaviour), true);
			EditorGUILayout.ObjectField("View", _viewProxy.View as Component, typeof(Component), true);
			EditorGUILayout.Toggle("Modal", _viewProxy.Modal);
			EditorGUILayout.Toggle("Exclusive", _viewProxy.Exclusive);

			EditorGUI.EndDisabledGroup();
		}
	}
}
