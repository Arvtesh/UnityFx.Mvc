// Copyright (C) 2019 Alexander Bogarsukov. All rights reserved.
// See the LICENSE.md file in the project root for more information.

using System;
using UnityEditor;
using UnityEngine;

namespace UnityFx.Mvc
{
	[CustomEditor(typeof(UGUIViewFactory.ViewProxy), true)]
	public class ViewProxyEditor : Editor
	{
		private UGUIViewFactory.ViewProxy _viewProxy;

		private void OnEnable()
		{
			_viewProxy = (UGUIViewFactory.ViewProxy)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUI.BeginDisabledGroup(true);

			EditorGUILayout.ObjectField("View", _viewProxy.View as Component, typeof(Component), true);
			EditorGUILayout.Toggle("Modal", _viewProxy.Modal);
			EditorGUILayout.Toggle("Exclusive", _viewProxy.Exclusive);

			EditorGUI.EndDisabledGroup();
		}
	}
}
