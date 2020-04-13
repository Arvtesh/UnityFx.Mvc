// Copyright (C) 2019 Alexander Bogarsukov. All rights reserved.
// See the LICENSE.md file in the project root for more information.

using System;
using UnityEditor;
using UnityEngine;

namespace UnityFx.Mvc
{
	[CustomEditor(typeof(UGUIMvcConfig))]
	public class UGUIMvcConfigEditor : MvcConfigEditor
	{
		protected override void InitPrefab(GameObject go)
		{
			base.InitPrefab(go);

			var t = go.AddComponent<RectTransform>();

			t.anchorMin = Vector2.zero;
			t.anchorMax = Vector2.one;
			t.anchoredPosition = Vector2.zero;
			t.sizeDelta = Vector2.zero;
		}
	}
}
