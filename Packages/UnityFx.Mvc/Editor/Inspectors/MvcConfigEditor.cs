// Copyright (C) 2019 Alexander Bogarsukov. All rights reserved.
// See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.Compilation;
using UnityEngine;

namespace UnityFx.Mvc
{
	[CustomEditor(typeof(MvcConfig))]
	public class MvcConfigEditor : Editor
	{
		private enum ControllerType
		{
			Default,
			Exclusive,
			Popup,
			ModalPopup
		}

		private MvcConfig _config;

		private SerializedProperty _viewControllers;
		private ReorderableList _viewControllersList;
		private SerializedProperty _folders;
		private ReorderableList _foldersList;
		private SerializedProperty _defaultNamespace;
		private SerializedProperty _baseControllerTypePath;
		private SerializedProperty _baseViewTypePath;

		private Regex _newControllerNamePattern = new Regex("^[_a-zA-Z][_a-zA-Z0-9]*$");
		private string _newControllerName;
		private string _newControllerNamespace;
		private string _newControllerPath;
		private bool _newControllerCreateArgs;
		private bool _newControllerCreateCommands;
		private ControllerType _newControllerType;

		private string _lastError;
		private bool _createControllerMode;
		private bool _controlsOpened;

		protected virtual void InitPrefab(GameObject go)
		{
			go.layer = LayerMask.NameToLayer("UI");
		}

		protected virtual void OnEnable()
		{
			_config = (MvcConfig)target;
			_defaultNamespace = serializedObject.FindProperty("_defaultNamespace");
			_baseControllerTypePath = serializedObject.FindProperty("_baseControllerTypePath");
			_baseViewTypePath = serializedObject.FindProperty("_baseViewTypePath");

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

			// Only show controls when not playing.
			if (!Application.isPlaying)
			{
				if (!string.IsNullOrEmpty(_lastError))
				{
					EditorGUILayout.Space();
					EditorGUILayout.HelpBox(_lastError, MessageType.Error);
				}

				EditorGUILayout.Space();
				EditorGUILayout.BeginHorizontal();
				{
					if (GUILayout.Button("Reset"))
					{
						OnResetBindings();
					}

					if (GUILayout.Button("Validate"))
					{
						OnValidateBindings();
					}

					if (GUILayout.Button("Clear"))
					{
						OnClearBindings();
					}
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();

				if (_createControllerMode)
				{
					PresentCreateControllerPanel();
				}
				else if (GUILayout.Button("Create New Controller..."))
				{
					OnCreateNewController();
				}

				EditorGUILayout.Space();

				if (_controlsOpened = EditorGUILayout.BeginFoldoutHeaderGroup(_controlsOpened, "Advanced settings"))
				{
					PresentAdvancedSettings();
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

		private void GenerateControllerCode(string assetPath, CodegenNames names, CodegenOptions options, string ns)
		{
			var numberOfSteps = 4;

			if ((options & CodegenOptions.CreateArgs) != 0)
			{
				++numberOfSteps;
			}

			if ((options & CodegenOptions.CreateCommands) != 0)
			{
				++numberOfSteps;
			}

			var title = "Generating code for " + _newControllerName;
			var step = 1f / numberOfSteps;
			var progress = 0f;

			try
			{
				var path = Path.Combine(assetPath, names.BaseName);
				var fullPath = Path.Combine(Directory.GetCurrentDirectory(), path);
				var baseControllerName = Path.GetFileNameWithoutExtension(_config.BaseControllerPath);
				var baseViewName = Path.GetFileNameWithoutExtension(_config.BaseViewPath);

				if (!Directory.Exists(fullPath))
				{
					Directory.CreateDirectory(fullPath);
				}

				var pathFolder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(path);
				var controllerPath = Path.Combine(path, names.ControllerFileName);
				var viewPath = Path.Combine(path, names.ViewFileName);
				var prefabPath = Path.Combine(path, names.PrefabFileName);
				var prefab = default(GameObject);

				// Controller
				EditorUtility.DisplayProgressBar(title, controllerPath, progress);
				File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), controllerPath), Codegen.GetControllerText(names, ns, baseControllerName, GetControllerTypeStr(_newControllerType), options));

				progress += step;

				// View
				EditorUtility.DisplayProgressBar(title, viewPath, progress);
				File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), viewPath), Codegen.GetViewText(names, ns, baseViewName, options));

				progress += step;

				// Args
				if ((options & CodegenOptions.CreateArgs) != 0)
				{
					var argsPath = Path.Combine(path, names.ArgsFileName);

					EditorUtility.DisplayProgressBar(title, argsPath, progress);
					File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), argsPath), Codegen.GetArgsText(names, ns, options));

					progress += step;
				}

				// Commands
				if ((options & CodegenOptions.CreateCommands) != 0)
				{
					var commandsPath = Path.Combine(path, names.CommandsFileName);

					EditorUtility.DisplayProgressBar(title, commandsPath, progress);
					File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), commandsPath), Codegen.GetCommandsText(names, ns, options));

					progress += step;
				}

				// Prefab
				var go = new GameObject(names.PrefabName);

				try
				{
					InitPrefab(go);
					prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(go, Path.Combine(path, names.PrefabFileName), InteractionMode.AutomatedAction);
				}
				finally
				{
					DestroyImmediate(go);
				}

				// Refresh assets
				EditorUtility.DisplayProgressBar(title, "Refreshing the assets..", progress);
				AssetDatabase.Refresh();

				// Compile the code
				EditorUtility.DisplayProgressBar(title, "Compiling code..", 1);
				CompilationPipeline.RequestScriptCompilation();
				CompilationPipeline.compilationFinished += OnCompilationFinished;

				_config.AddItem(new MvcConfig.ViewControllerInfo()
				{
					ControllerScriptPath = controllerPath,
					ViewScriptPath = viewPath,
					ViewResourceId = names.ControllerName,
					ViewPrefab = prefab
				});
			}
			catch (Exception e)
			{
				EditorUtility.ClearProgressBar();
				Debug.LogException(e);
			}
		}

		private void OnCompilationFinished(object obj)
		{
			CompilationPipeline.compilationFinished -= OnCompilationFinished;
			EditorUtility.ClearProgressBar();
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

			var itemCx = rect.width / 2 - padding;
			var item = _viewControllersList.serializedProperty.GetArrayElementAtIndex(index);
			var controllerProp = item.FindPropertyRelative("ControllerScriptPath");
			var controllerObj = AssetDatabase.LoadAssetAtPath<MonoScript>(controllerProp.stringValue);

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

			// Prefab reference.
			var prefabProp = item.FindPropertyRelative("ViewPrefab");
			var prefabObj = prefabProp.objectReferenceValue;
			var newPrefabObj = EditorGUI.ObjectField(
				new Rect(rect.x + itemCx + padding, rect.y + 1, itemCx + padding, EditorGUIUtility.singleLineHeight),
				prefabObj,
				typeof(GameObject),
				false);

			if (newPrefabObj != prefabObj)
			{
				TryAssignPrefab(item, (GameObject)newPrefabObj, prefabProp, controllerObj?.GetClass());
			}
		}

		private void OnDrawViewControllerHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, "Controller bindings");
		}

		private void OnViewControllerAdd(ReorderableList list)
		{
			Undo.RecordObject(target, "Add Item");
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

		private void OnResetBindings()
		{
			Undo.RecordObject(target, "Reset Bindings");

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

		private void OnValidateBindings()
		{
			Undo.RecordObject(target, "Validate Bindings");
			_lastError = string.Empty;

			for (var i = 0; i < _config.ViewControllers.Count; ++i)
			{
				var item = _config.ViewControllers[i];

				if (!item.ViewPrefab || string.IsNullOrWhiteSpace(item.ViewResourceId))
				{
					_config.ViewControllers.RemoveAt(i--);
				}
			}
		}

		private void OnClearBindings()
		{
			Undo.RecordObject(target, "Clear Bindings");
			_lastError = string.Empty;
			_config.Clear();
		}

		private void OnCreateNewController()
		{
			_newControllerPath = AssetDatabase.GetAssetPath(target);
			_newControllerPath = Path.GetDirectoryName(_newControllerPath);

			if (!string.IsNullOrEmpty(_defaultNamespace.stringValue))
			{
				_newControllerNamespace = _defaultNamespace.stringValue;
			}

			_newControllerName = null;
			_createControllerMode = true;
		}

		private void PresentCreateControllerPanel()
		{
			var basePath = string.IsNullOrWhiteSpace(_newControllerName) ? _newControllerPath : Path.Combine(_newControllerPath, _newControllerName);
			var validateResult = true;

			EditorGUILayout.BeginVertical();
			EditorGUILayout.BeginFoldoutHeaderGroup(true, "Create new controller...");

			// New controller path.
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.TextField("Path", basePath);
			EditorGUI.EndDisabledGroup();

			_newControllerName = EditorGUILayout.TextField("Controller Name", _newControllerName);
			_newControllerNamespace = EditorGUILayout.TextField("Namespace", _newControllerNamespace);
			_newControllerCreateArgs = EditorGUILayout.Toggle("Create PresentArgs", _newControllerCreateArgs);
			_newControllerCreateCommands = EditorGUILayout.Toggle("Create commands enumeration", _newControllerCreateCommands);
			_newControllerType = (ControllerType)EditorGUILayout.EnumPopup("Controller type", _newControllerType);

			if (string.IsNullOrWhiteSpace(_newControllerName) || !_newControllerNamePattern.IsMatch(_newControllerName))
			{
				validateResult = false;
				EditorGUILayout.HelpBox($"Invalid controller name. Controller names should match pattern {_newControllerNamePattern}.", MessageType.Error);
			}
			else
			{
				var folder = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine(_newControllerPath, _newControllerName));

				if (Directory.Exists(folder))
				{
					validateResult = false;
					EditorGUILayout.HelpBox($"Folder with name {_newControllerName} already exists. Please choose another controller name.", MessageType.Error);
				}
			}

			if (!string.IsNullOrEmpty(_newControllerNamespace) && !_newControllerNamePattern.IsMatch(_newControllerNamespace))
			{
				validateResult = false;
				EditorGUILayout.HelpBox($"Invalid namespace name. Namespace names should match pattern {_newControllerNamePattern} (or an empty string).", MessageType.Error);
			}

			if (validateResult)
			{
				var controllerPath = Path.Combine(basePath, _newControllerName + "Controller.cs");
				var viewPath = Path.Combine(basePath, _newControllerName + "View.cs");
				var prefabPath = Path.Combine(basePath, _newControllerName + ".prefab");
				var s = $"Assets to create:\n    - {controllerPath}\n    - {viewPath}";

				if (_newControllerCreateArgs)
				{
					var argsPath = Path.Combine(basePath, _newControllerName + "Args.cs");
					s += "\n    - " + argsPath;
				}

				if (_newControllerCreateCommands)
				{
					var argsPath = Path.Combine(basePath, _newControllerName + "Commands.cs");
					s += "\n    - " + argsPath;
				}

				s += "\n    - " + prefabPath;

				EditorGUILayout.HelpBox(s, MessageType.Info);
				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button("Create assets"))
				{
					var options = default(CodegenOptions);

					if (_newControllerCreateArgs)
					{
						options |= CodegenOptions.CreateArgs;
					}

					if (_newControllerCreateCommands)
					{
						options |= CodegenOptions.CreateCommands;
					}

					GenerateControllerCode(_newControllerPath, new CodegenNames(_newControllerName), options, _newControllerNamespace);
					_createControllerMode = false;
				}

				if (GUILayout.Button("Cancel"))
				{
					_createControllerMode = false;
				}

				EditorGUILayout.EndHorizontal();
			}
			else
			{
				if (GUILayout.Button("Close"))
				{
					_createControllerMode = false;
				}
			}

			EditorGUILayout.EndFoldoutHeaderGroup();
			EditorGUILayout.EndVertical();
		}

		private void PresentAdvancedSettings()
		{
			EditorGUILayout.PropertyField(_defaultNamespace);

			var controllerObj = AssetDatabase.LoadAssetAtPath<MonoScript>(_baseControllerTypePath.stringValue);
			var newControllerObj = EditorGUILayout.ObjectField(
				"Default Controller",
				controllerObj,
				typeof(MonoScript),
				false);

			if (newControllerObj != controllerObj)
			{
				if (newControllerObj)
				{
					var controllerType = ((MonoScript)newControllerObj).GetClass();

					if (typeof(IViewController).IsAssignableFrom(controllerType))
					{
						_baseControllerTypePath.stringValue = AssetDatabase.GetAssetPath(newControllerObj);
					}
					else
					{
						_baseControllerTypePath.stringValue = MvcConfig.DefaultControllerPath;
					}
				}
				else
				{
					_baseControllerTypePath.stringValue = MvcConfig.DefaultControllerPath;
				}
			}

			var viewObj = AssetDatabase.LoadAssetAtPath<MonoScript>(_baseViewTypePath.stringValue);
			var newViewObj = EditorGUILayout.ObjectField(
				"Default View",
				viewObj,
				typeof(MonoScript),
				false);

			if (newViewObj != viewObj)
			{
				if (newViewObj)
				{
					var viewType = ((MonoScript)newViewObj).GetClass();

					if (typeof(IView).IsAssignableFrom(viewType))
					{
						_baseViewTypePath.stringValue = AssetDatabase.GetAssetPath(newViewObj);
					}
					else
					{
						_baseViewTypePath.stringValue = MvcConfig.DefaultViewPath;
					}
				}
				else
				{
					_baseViewTypePath.stringValue = MvcConfig.DefaultViewPath;
				}
			}

			EditorGUILayout.Space();
			_foldersList.DoLayoutList();
		}

		private bool TryAssignPrefab(SerializedProperty item, GameObject prefab, SerializedProperty prefabProp, Type controllerType)
		{
			var resourceIdProp = item.FindPropertyRelative("ViewResourceId");
			var viewProp = item.FindPropertyRelative("ViewScriptPath");
			var viewObj = AssetDatabase.LoadAssetAtPath<MonoScript>(viewProp.stringValue);

			if (prefab)
			{
				var validateResult = true;

				if (controllerType != null)
				{
					// Validate that prefab has the same view script assigned as controller requires.
					if (viewObj)
					{
						var viewType = viewObj.GetClass();

						if (viewType != null)
						{
							var view = prefab.GetComponent(viewType);

							if (view is null)
							{
								validateResult = false;
								_lastError = $"The prefab '{prefab.name}' doesn't have component of type {viewType.Name} attached and, thus, cannot be set as view for {controllerType.Name}.";
							}
						}
					}
				}
				else
				{
					// If no controller assigned, search for the view asset.
					var view = prefab.GetComponent(typeof(IView)) as MonoBehaviour;

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

					prefabProp.objectReferenceValue = prefab;

					if (controllerType != null)
					{
						resourceIdProp.stringValue = MvcConfig.GetResourceId(controllerType);
					}
					else
					{
						resourceIdProp.stringValue = prefab.name;
					}

					return true;
				}
			}
			else
			{
				_lastError = string.Empty;

				prefabProp.objectReferenceValue = null;
				resourceIdProp.stringValue = null;

				if (controllerType == null)
				{
					viewProp.stringValue = null;
				}

				return true;
			}

			return false;
		}

		private static string GetControllerTypeStr(ControllerType controllerType)
		{
			switch (controllerType)
			{
				case ControllerType.Exclusive:
					return nameof(ViewControllerFlags) + '.' + nameof(ViewControllerFlags.Exclusive);

				case ControllerType.Popup:
					return nameof(ViewControllerFlags) + '.' + nameof(ViewControllerFlags.Popup);

				case ControllerType.ModalPopup:
					return nameof(ViewControllerFlags) + '.' + nameof(ViewControllerFlags.ModalPopup);
			}

			return null;
		}
	}
}
