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
		private SerializedProperty _folders;
		private ReorderableList _foldersList;

		private string _lastError;
		private string _lastWarning;
		private bool _controlsOpened;

		protected virtual void OnEnable()
		{
			_config = (MvcConfig)target;

			// https://blog.terresquall.com/2020/03/creating-reorderable-lists-in-the-unity-inspector/
			_viewControllers = serializedObject.FindProperty("_viewControllers");
			_viewControllersList = new ReorderableList(serializedObject, _viewControllers, true, true, true, true);
			_viewControllersList.drawElementCallback += OnDrawViewControllerInfo;
			_viewControllersList.drawHeaderCallback += OnDrawViewControllerHeader;
			_viewControllersList.onAddCallback += OnViewControllerAdd;

			_folders = serializedObject.FindProperty("_folders");
			_foldersList = new ReorderableList(serializedObject, _folders, true, true, true, true);
			_foldersList.drawElementCallback += OnDrawFolderInfo;
			_foldersList.drawHeaderCallback += OnDrawFolderHeader;
		}

		protected virtual void OnGUI()
		{
			// List.
			_viewControllersList.DoLayoutList();

			// Onlu show controls when not playing.
			if (!Application.isPlaying)
			{
				// Error message.
				if (!string.IsNullOrEmpty(_lastError))
				{
					EditorGUILayout.Space();
					EditorGUILayout.HelpBox(_lastError, MessageType.Error);
				}

				// Buttons.
				EditorGUILayout.Space();
				EditorGUILayout.BeginHorizontal();
				{
					if (GUILayout.Button("Reset"))
					{
						_lastWarning = string.Empty;
						_lastError = string.Empty;
						_config.Clear();

						if (_config.SearchFolders != null && _config.SearchFolders.Count > 0)
						{
							foreach (var folder in _config.SearchFolders)
							{
								AddViewControllersFromPath(folder);
							}
						}
						else
						{
							var configPath = AssetDatabase.GetAssetPath(target);

							if (!string.IsNullOrEmpty(configPath))
							{
								configPath = Path.GetDirectoryName(configPath);
								AddViewControllersFromPath(configPath);
							}
						}
					}

					if (GUILayout.Button("Validate"))
					{
						_lastWarning = string.Empty;
						_lastError = string.Empty;

						// TODO
					}

					if (GUILayout.Button("Clear"))
					{
						_lastWarning = string.Empty;
						_lastError = string.Empty;
						_config.Clear();
					}
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();

				// Advanced controls.
				if (_controlsOpened = EditorGUILayout.BeginFoldoutHeaderGroup(_controlsOpened, "Advanced settings"))
				{
					_foldersList.DoLayoutList();
				}

				EditorGUILayout.EndFoldoutHeaderGroup();
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
						ViewResourceId = MvcConfig.GetResourceId(controllerType),
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
			var viewPathProp = item.FindPropertyRelative("ViewResourceId");

			// Controller script.
			EditorGUI.BeginDisabledGroup(true);
			{
				if (controllerObj)
				{
					EditorGUI.ObjectField(
						new Rect(rect.x, rect.y + 1, itemCx, EditorGUIUtility.singleLineHeight),
						controllerObj,
						typeof(MonoScript),
						false);
				}
			}
			EditorGUI.EndDisabledGroup();

			// View script (read-only).
			EditorGUI.BeginDisabledGroup(true);
			{
				if (viewObj)
				{
					EditorGUI.ObjectField(
						new Rect(rect.x + itemCx + padding, rect.y + 1, itemCx, EditorGUIUtility.singleLineHeight),
						viewObj,
						typeof(MonoScript),
						false);
				}
			}
			EditorGUI.EndDisabledGroup();

			// Prefab reference.
			var newPrefabObj = EditorGUI.ObjectField(
				new Rect(rect.x + 2 * (itemCx + padding), rect.y + 1, itemCx + padding, EditorGUIUtility.singleLineHeight),
				prefabObj,
				typeof(GameObject),
				false);

			if (newPrefabObj != prefabObj)
			{
				if (newPrefabObj)
				{
					var validateResult = true;

					if (controllerObj)
					{
						// Validate that prefab has the same view script assigned as controller requires.
						if (viewObj)
						{
							var viewType = viewObj.GetClass();

							if (viewType != null)
							{
								var view = ((GameObject)newPrefabObj).GetComponent(viewType);

								if (view is null)
								{
									validateResult = false;
									_lastError = $"The prefab '{newPrefabObj.name}' doesn't have component of type {viewType.Name} attached and, thus, cannot be set as view for {controllerObj.name}.";
								}
							}
						}
					}
					else
					{
						// If no controller assigned, search for the view asset.
						var view = ((GameObject)newPrefabObj).GetComponent(typeof(IView)) as MonoBehaviour;

						if (view)
						{
							var viewScript = MonoScript.FromMonoBehaviour(view);

							if (viewScript)
							{
								viewProp.stringValue = AssetDatabase.GetAssetPath(viewScript);
							}
							else
							{
								viewProp.stringValue = null;
							}
						}
						else
						{
							viewProp.stringValue = null;
						}
					}

					if (validateResult)
					{
						_lastError = string.Empty;

						prefabProp.objectReferenceValue = newPrefabObj;

						if (controllerObj)
						{
							viewPathProp.stringValue = MvcConfig.GetResourceId(controllerObj.GetClass());
						}
						else
						{
							viewPathProp.stringValue = newPrefabObj.name;
						}
					}
				}
				else
				{
					_lastError = string.Empty;

					prefabProp.objectReferenceValue = null;
					viewPathProp.stringValue = null;

					if (!controllerObj)
					{
						viewProp.stringValue = null;
					}
				}
			}
		}

		private void OnDrawViewControllerHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, "ViewController bindings");
		}

		private void OnViewControllerAdd(ReorderableList list)
		{
			_config.AddItem(new MvcConfig.ViewControllerInfo());
		}

		private void OnDrawFolderInfo(Rect rect, int index, bool isActive, bool isFocused)
		{
			var folderProp = _foldersList.serializedProperty.GetArrayElementAtIndex(index);
			var folderObj = AssetDatabase.LoadAssetAtPath<DefaultAsset>(folderProp.stringValue);
			var newFolderObj = EditorGUI.ObjectField(
				new Rect(rect.x, rect.y + 1, rect.width, EditorGUIUtility.singleLineHeight),
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

		private void OnDrawFolderHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, "Search folders");
		}
	}
}
