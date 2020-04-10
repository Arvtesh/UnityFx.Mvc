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
		private SerializedProperty _bgColor;
		private SerializedProperty _prefabs;
		private ReorderableList _prefabsList;

		private SerializedProperty _customPrefabs;
		private ReorderableList _customPrefabsList;
		private SerializedProperty _prefabsGroups;
		private ReorderableList _prefabsGroupsList;
		private bool _controlsOpened;

		private void OnEnable()
		{
			_config = (UGUIViewFactoryConfig)target;
			_bgColor = serializedObject.FindProperty("_popupBackgroundColor");

			// https://blog.terresquall.com/2020/03/creating-reorderable-lists-in-the-unity-inspector/
			_prefabs = serializedObject.FindProperty("_prefabs");
			_prefabsList = new ReorderableList(serializedObject, _prefabs, true, true, true, true);
			_prefabsList.drawElementCallback += OnDrawPrefab;
			_prefabsList.drawHeaderCallback += OnDrawPrefabHeader;

			_customPrefabs = serializedObject.FindProperty("_customPrefabs");
			_customPrefabsList = new ReorderableList(serializedObject, _customPrefabs, true, true, true, true);
			_customPrefabsList.drawElementCallback += OnDrawCustomPrefab;
			_customPrefabsList.drawHeaderCallback += OnDrawCustomPrefabHeader;

			_prefabsGroups = serializedObject.FindProperty("_prefabGroups");
			_prefabsGroupsList = new ReorderableList(serializedObject, _prefabsGroups, true, true, true, true);
			_prefabsGroupsList.drawElementCallback += OnDrawPrefabGroup;
			_prefabsGroupsList.drawHeaderCallback += OnDrawPrefabGroupHeader;
		}

		public override void OnInspectorGUI()
		{
			EditorGUI.BeginDisabledGroup(Application.isPlaying);
			{
				base.OnInspectorGUI();

				EditorGUILayout.Space();
				_prefabsList.DoLayoutList();
			}
			EditorGUI.EndDisabledGroup();

			if (!Application.isPlaying)
			{
				EditorGUILayout.Space();

				_controlsOpened = EditorGUILayout.BeginFoldoutHeaderGroup(_controlsOpened, "Prefab controls");

				if (_controlsOpened)
				{
					EditorGUILayout.HelpBox("Use these controls to quickly manage content of Prefabs. Pressing Reset Prefabs button recursively searches for UGUI prefabs in folders specified at Prefab Groups. Content of Custom Prefabs is added as is afterwards.", MessageType.Info);
					EditorGUILayout.Space();

					_prefabsGroupsList.DoLayoutList();
					_customPrefabsList.DoLayoutList();

					if (GUILayout.Button("Clear Prefabs"))
					{
						_config.ClearPrefabs();
					}

					if (GUILayout.Button("Reset Prefabs"))
					{
						_config.ClearPrefabs();

						if (_config.PrefabGroups.Count == 0)
						{
							var configPath = AssetDatabase.GetAssetPath(target);

							if (!string.IsNullOrEmpty(configPath))
							{
								configPath = Path.GetDirectoryName(configPath);
								AddPrefabsFroGroup(configPath, null);
							}
						}
						else
						{
							foreach (var prefabGroup in _config.PrefabGroups)
							{
								AddPrefabsFroGroup(prefabGroup.Folder, prefabGroup.PathPrefix);
							}
						}

						foreach (var prefabDesc in _config.CustomPrefabs)
						{
							_config.AddPrefab(prefabDesc);
						}
					}
				}

				EditorGUILayout.EndFoldoutHeaderGroup();
			}

			serializedObject.ApplyModifiedProperties();
		}

		private void AddPrefabsFroGroup(string folder, string prefix)
		{
			var pathPrefix = Application.dataPath;
			pathPrefix = pathPrefix.Substring(0, pathPrefix.Length - 6);

			var fullPath = Path.Combine(pathPrefix, folder);

			if (Directory.Exists(fullPath))
			{
				var prefabPaths = Directory.GetFiles(fullPath, "*.prefab", SearchOption.AllDirectories);

				foreach (var prefabPath in prefabPaths)
				{
					var localPath = prefabPath.Replace(pathPrefix, string.Empty);
					var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(localPath);

					if (prefab && prefab.GetComponent<IView>() != null)
					{
						_config.AddPrefab(prefix, prefab);
					}
				}
			}
		}

		private void DrawPrefab(SerializedProperty item, Rect rect, bool isActive, bool isFocused)
		{
			const int cx1 = 40;
			const int cx2 = 50;

			var cx = rect.width / 2;

			EditorGUI.LabelField(new Rect(rect.x, rect.y, cx1, EditorGUIUtility.singleLineHeight), "Path");
			EditorGUI.PropertyField(
				new Rect(rect.x + cx1, rect.y + 1, cx - cx1, EditorGUIUtility.singleLineHeight),
				item.FindPropertyRelative("Path"),
				GUIContent.none);

			EditorGUI.LabelField(new Rect(rect.x + cx + 5, rect.y, cx2, EditorGUIUtility.singleLineHeight), "Prefab");
			EditorGUI.PropertyField(
				new Rect(rect.x + cx + cx2, rect.y + 1, rect.width - cx - cx2, EditorGUIUtility.singleLineHeight),
				item.FindPropertyRelative("Prefab"),
				GUIContent.none);
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

		private void OnDrawPrefab(Rect rect, int index, bool isActive, bool isFocused)
		{
			var item = _prefabsList.serializedProperty.GetArrayElementAtIndex(index);

			DrawPrefab(item, rect, isActive, isFocused);
		}

		private void OnDrawCustomPrefab(Rect rect, int index, bool isActive, bool isFocused)
		{
			var item = _customPrefabsList.serializedProperty.GetArrayElementAtIndex(index);

			DrawPrefab(item, rect, isActive, isFocused);
		}

		private void OnDrawPrefabGroupHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, "Prefab Groups");
		}

		private void OnDrawPrefabHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, "Prefabs");
		}

		private void OnDrawCustomPrefabHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, "Custom Prefabs");
		}
	}
}
