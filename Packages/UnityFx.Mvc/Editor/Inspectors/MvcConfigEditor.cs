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
	[CustomEditor(typeof(MvcConfig))]
	public class MvcConfigEditor : Editor
	{
		private MvcConfig _config;
		private SerializedProperty _viewControllers;
		private ReorderableList _viewControllersList;
		private string _lastError;

		private void OnEnable()
		{
			_config = (MvcConfig)target;

			// https://blog.terresquall.com/2020/03/creating-reorderable-lists-in-the-unity-inspector/
			_viewControllers = serializedObject.FindProperty("_viewControllers");
			_viewControllersList = new ReorderableList(serializedObject, _viewControllers, true, true, true, true);
			_viewControllersList.drawElementCallback += OnDrawViewControllerInfo;
			_viewControllersList.drawHeaderCallback += OnDrawViewControllerHeader;
		}

		protected virtual void OnGUI()
		{
			_viewControllersList.DoLayoutList();

			if (!Application.isPlaying)
			{
				EditorGUILayout.Space();

				if (GUILayout.Button("Clear"))
				{
					_config.Clear();
				}

				if (GUILayout.Button("Reset"))
				{
					_config.Clear();

					{
						var configPath = AssetDatabase.GetAssetPath(target);

						if (!string.IsNullOrEmpty(configPath))
						{
							configPath = Path.GetDirectoryName(configPath);
							AddViewControllersFromPath(configPath);
						}
					}
				}

				if (!string.IsNullOrEmpty(_lastError))
				{
					EditorGUILayout.Space();
					EditorGUILayout.HelpBox(_lastError, MessageType.Error);
				}
			}
		}

		public sealed override void OnInspectorGUI()
		{
			EditorGUI.BeginDisabledGroup(Application.isPlaying);
			{
				base.OnInspectorGUI();

				EditorGUILayout.Space();
				OnGUI();
			}
			EditorGUI.EndDisabledGroup();

			serializedObject.ApplyModifiedProperties();
		}

		private void AddViewControllersFromPath(string path)
		{
			var controllers = new Dictionary<Type, MonoScript>();
			var views = new Dictionary<Type, MonoScript>();
			var viewPrefabs = new Dictionary<Type, List<GameObject>>();
			var allPrefabs = new List<GameObject>();

			var pathPrefix = Application.dataPath;
			pathPrefix = pathPrefix.Substring(0, pathPrefix.Length - 6);

			var fullPath = Path.Combine(pathPrefix, path);

			if (Directory.Exists(fullPath))
			{
				// Search for controllers and views.
				var paths = Directory.GetFiles(fullPath, "*.cs", SearchOption.AllDirectories);

				foreach (var scriptPath in paths)
				{
					var localPath = scriptPath.Replace(pathPrefix, string.Empty);
					var script = AssetDatabase.LoadAssetAtPath<MonoScript>(localPath);

					if (script)
					{
						var type = script.GetClass();

						if (type != null)
						{
							if (typeof(IViewController).IsAssignableFrom(type))
							{
								controllers.Add(type, script);
							}
							else if (typeof(IView).IsAssignableFrom(type))
							{
								views.Add(type, script);
							}
						}
					}
				}

				// Search for prefabs.
				paths = Directory.GetFiles(fullPath, "*.prefab", SearchOption.AllDirectories);

				foreach (var prefabPath in paths)
				{
					var localPath = prefabPath.Replace(pathPrefix, string.Empty);
					var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(localPath);

					if (prefab)
					{
						var view = prefab.GetComponent<IView>();

						if (view != null)
						{
							var type = view.GetType();

							if (viewPrefabs.TryGetValue(type, out var list))
							{
								list.Add(prefab);
							}
							else
							{
								viewPrefabs.Add(type, new List<GameObject>() { prefab });
							}
						}

						allPrefabs.Add(prefab);
					}
				}

				// Fill the config.
				foreach (var kvp in controllers)
				{
					var controllerType = kvp.Key;
					var controllerScript = kvp.Value;
					var controllerScriptPath = AssetDatabase.GetAssetPath(controllerScript);
					var viewScript = default(MonoScript);
					var viewPrefab = default(GameObject);

					if (MvcUtilities.IsAssignableToGenericType(controllerType, typeof(IViewControllerView<>), out var t))
					{
						var viewType = t.GenericTypeArguments[0];

						views.TryGetValue(viewType, out viewScript);

						if (viewPrefabs.TryGetValue(viewType, out var prefabList))
						{
							if (prefabList.Count == 1)
							{
								viewPrefab = prefabList[0];
							}
							else
							{
								// TODO
							}
						}
					}

					if (viewScript is null)
					{
						// TODO
					}

					if (viewPrefab is null)
					{
						var controllerFolder = Path.GetDirectoryName(controllerScriptPath);
						var controllerName = MvcUtilities.GetControllerName(controllerType);

						foreach (var go in allPrefabs)
						{
							// TODO: Compare view path to controller path.
							if (string.CompareOrdinal(controllerName, go.name) == 0)
							{
								viewPrefab = go;
								break;
							}
						}
					}

					// Add the new item.
					_config.AddItem(new MvcConfig.ViewControllerInfo()
					{
						ControllerScriptPath = controllerScriptPath,
						ViewScriptPath = AssetDatabase.GetAssetPath(viewScript),
						ViewPath = AssetDatabase.GetAssetPath(viewPrefab),
						ViewPrefab = viewPrefab
					});
				}
			}
		}

		private void OnDrawViewControllerInfo(Rect rect, int index, bool isActive, bool isFocused)
		{
			const int padding = 5;

			var itemCx = rect.width / 3 - padding;
			var item = _viewControllersList.serializedProperty.GetArrayElementAtIndex(index);
			var controllerProp = item.FindPropertyRelative("ControllerScriptPath");
			var controllerObj = AssetDatabase.LoadAssetAtPath<MonoScript>(controllerProp.stringValue);
			var viewProp = item.FindPropertyRelative("ViewScriptPath");
			var viewObj = AssetDatabase.LoadAssetAtPath<MonoScript>(viewProp.stringValue);
			var prefabProp = item.FindPropertyRelative("ViewPrefab");
			var prefabObj = prefabProp.objectReferenceValue;
			var vewPathProp = item.FindPropertyRelative("ViewPath");

			var newControllerObj = EditorGUI.ObjectField(
				new Rect(rect.x, rect.y + 1, itemCx, EditorGUIUtility.singleLineHeight),
				controllerObj,
				typeof(MonoScript),
				false);

			if (newControllerObj != controllerObj)
			{
				if (newControllerObj)
				{
					// TODO: Validate the script type.
					controllerProp.stringValue = AssetDatabase.GetAssetPath(newControllerObj);
				}
				else
				{
					controllerProp.stringValue = string.Empty;
				}
			}

			var newViewObj = EditorGUI.ObjectField(
				new Rect(rect.x + itemCx + padding, rect.y + 1, itemCx, EditorGUIUtility.singleLineHeight),
				viewObj,
				typeof(MonoScript),
				false);

			if (newViewObj != viewObj)
			{
				if (newViewObj)
				{
					// TODO: Validate the script type.
					viewProp.stringValue = AssetDatabase.GetAssetPath(newViewObj);
				}
				else
				{
					viewProp.stringValue = string.Empty;
				}
			}

			var newPrefabObj = EditorGUI.ObjectField(
				new Rect(rect.x + 2 * (itemCx + padding), rect.y + 1, itemCx + padding, EditorGUIUtility.singleLineHeight),
				prefabObj,
				typeof(GameObject),
				false);

			if (newPrefabObj != prefabObj)
			{
				if (newPrefabObj)
				{
					// TODO: Validate the object type.
					prefabProp.objectReferenceValue = newPrefabObj;
				}
				else
				{
					prefabProp.objectReferenceValue = null;
				}
			}
		}

		private void OnDrawViewControllerHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, "ViewControllers");
		}
	}
}
