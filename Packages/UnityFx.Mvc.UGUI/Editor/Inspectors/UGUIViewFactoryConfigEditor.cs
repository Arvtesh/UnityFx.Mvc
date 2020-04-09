// Copyright (C) 2019 Alexander Bogarsukov. All rights reserved.
// See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UnityFx.Mvc
{
	[CustomEditor(typeof(UGUIViewFactoryConfig))]
	public class UGUIViewFactoryConfigEditor : Editor
	{
		private UGUIViewFactoryConfig _config;
		private SerializedProperty _prefabsGroups;
		private ReorderableList _prefabsGroupsList;
		private bool _prefabsOpened;

		private void OnEnable()
		{
			_config = (UGUIViewFactoryConfig)target;

			// https://blog.terresquall.com/2020/03/creating-reorderable-lists-in-the-unity-inspector/
			_prefabsGroups = serializedObject.FindProperty("_prefabGroups");
			_prefabsGroupsList = new ReorderableList(serializedObject, _prefabsGroups, true, true, true, true);
			_prefabsGroupsList.drawElementCallback += OnDrawPrefabGroup;
			_prefabsGroupsList.drawHeaderCallback += OnDrawPrefabGroupHeader;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUILayout.Space();

			_prefabsGroupsList.DoLayoutList();

			if (GUILayout.Button("Clear Prefabs"))
			{
				_config.Prefabs.Clear();
			}

			if (GUILayout.Button("Reset Prefabs"))
			{
				_config.Prefabs.Clear();

				foreach (var prefabGroup in _config.PrefabGroups)
				{
					var pathPrefix = Application.dataPath;
					pathPrefix = pathPrefix.Substring(0, pathPrefix.Length - 6);

					var fullPath = Path.Combine(pathPrefix, prefabGroup.Folder);
					var prefabPaths = Directory.GetFiles(fullPath, "*.prefab", SearchOption.AllDirectories);

					foreach (var prefabPath in prefabPaths)
					{
						var localPath = prefabPath.Replace(pathPrefix, string.Empty);
						var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(localPath);

						if (prefab && prefab.GetComponent<IView>() != null)
						{
							_config.Prefabs.Add(new UGUIViewFactoryConfig.PrefabDesc()
							{
								Path = string.IsNullOrEmpty(prefabGroup.PathPrefix) ? prefab.name : prefabGroup.PathPrefix + '/' + prefab.name,
								Prefab = prefab
							});
						}
					}
				}
			}

			EditorGUI.BeginDisabledGroup(true);

			_prefabsOpened = EditorGUILayout.Foldout(_prefabsOpened, "Prefabs", true);

			if (_prefabsOpened)
			{
				EditorGUI.indentLevel += 1;

				if (_config.Prefabs == null || _config.Prefabs.Count == 0)
				{
					EditorGUILayout.LabelField("Empty");
				}
				else
				{
					foreach (var prefabDesc in _config.Prefabs)
					{
						EditorGUILayout.ObjectField(prefabDesc.Path, prefabDesc.Prefab, typeof(GameObject), false);
					}
				}

				EditorGUI.indentLevel -= 1;
			}

			EditorGUI.EndDisabledGroup();

			serializedObject.ApplyModifiedProperties();
		}

		private void OnDrawPrefabGroup(Rect rect, int index, bool isActive, bool isFocused)
		{
			const int cx1 = 40;
			const int cx2 = 50;

			var item = _prefabsGroupsList.serializedProperty.GetArrayElementAtIndex(index);
			var folderProp = item.FindPropertyRelative("Folder");
			var folderObj = AssetDatabase.LoadAssetAtPath<DefaultAsset>(folderProp.stringValue);
			var cx = rect.width / 2;

			EditorGUI.LabelField(new Rect(rect.x, rect.y, cx1, EditorGUIUtility.singleLineHeight), "Prefix");
			EditorGUI.PropertyField(
				new Rect(rect.x + cx1, rect.y + 1, cx - cx1, EditorGUIUtility.singleLineHeight),
				item.FindPropertyRelative("PathPrefix"),
				GUIContent.none);

			EditorGUI.LabelField(new Rect(rect.x + cx + 5, rect.y, cx2, EditorGUIUtility.singleLineHeight), "Folder");

			var newFolderObj = EditorGUI.ObjectField(
				new Rect(rect.x + cx + cx2, rect.y + 1, rect.width - cx - cx2, EditorGUIUtility.singleLineHeight),
				folderObj,
				typeof(DefaultAsset),
				false);

			if (newFolderObj != folderObj)
			{
				if (newFolderObj)
				{
					folderProp.stringValue = AssetDatabase.GetAssetPath(newFolderObj);
				}
				else
				{
					folderProp.stringValue = string.Empty;
				}
			}
		}

		private void OnDrawPrefabGroupHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, "Prefab Groups");
		}
	}
}
