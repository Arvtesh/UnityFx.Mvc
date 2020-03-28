// Copyright (C) 2019 Alexander Bogarsukov. All rights reserved.
// See the LICENSE.md file in the project root for more information.

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace UnityFx.Mvc
{
	public class CreateViewControllerWindow : EditorWindow
	{
		private string _basePath;
		private string _controllerName;
		private string _namespace;
		private string _controllerBaseClass = nameof(ViewController);
		private string _viewBaseClass = nameof(View);
		private bool _exclusive;
		private bool _modal;
		private bool _popup;
		private bool _createArgs;
		private bool _createCommands;

		private Regex _classNamePattern = new Regex("^[_a-zA-Z][_a-zA-Z0-9]*$");

		[MenuItem("Assets/Create/UnityFx/New ViewController...")]
		public static void Init()
		{
			GetWindow<CreateViewControllerWindow>("New ViewController...", true).Show();
		}

		private void OnGUI()
		{
			_basePath = GetSelectedPath();

			if (string.IsNullOrEmpty(_basePath))
			{
				EditorGUILayout.HelpBox("Invalid path. Please select an asset folder.", MessageType.Error);
				return;
			}

			GUI.enabled = false;
			EditorGUILayout.TextField("Base path", _basePath);
			GUI.enabled = true;

			_controllerName = EditorGUILayout.TextField("Controller Name", _controllerName);
			_createArgs = EditorGUILayout.Toggle("Create PresentArgs", _createArgs);
			_createCommands = EditorGUILayout.Toggle("Create commands enumeration", _createCommands);
			_exclusive = EditorGUILayout.Toggle("Exclusive", _exclusive);
			_popup = EditorGUILayout.Toggle("Popup", _popup);
			_modal = EditorGUILayout.Toggle("Modal", _modal);
			_namespace = EditorGUILayout.TextField("Namespace", _namespace);
			_controllerBaseClass = EditorGUILayout.TextField("Controller Base Class", _controllerBaseClass);
			_viewBaseClass = EditorGUILayout.TextField("View Base Class", _viewBaseClass);

			if (string.IsNullOrWhiteSpace(_controllerName) || !_classNamePattern.IsMatch(_controllerName))
			{
				EditorGUILayout.HelpBox($"Invalid controller name. Controller names should match {_classNamePattern}.", MessageType.Error);
			}

			if (_exclusive)
			{
				_popup = false;
				_modal = false;
			}

			if (_modal)
			{
				_popup = true;
			}

			if (string.IsNullOrEmpty(_controllerBaseClass))
			{
				_controllerBaseClass = nameof(ViewController);
			}

			if (string.IsNullOrEmpty(_viewBaseClass))
			{
				_viewBaseClass = nameof(View);
			}

			if (GUILayout.Button("Create"))
			{
				CreateViewController(_basePath);
			}
		}

		private void CreateViewController(string path)
		{
			if (string.IsNullOrEmpty(_controllerName))
			{
				Debug.LogError($"Invalid controller name '{_controllerName}'.");
			}
			else if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
			{
				Debug.LogError($"Invalid controller path: '{path}'.");
			}
			else
			{
				path = Path.Combine(path, _controllerName);

				var fullPath = Path.Combine(Directory.GetCurrentDirectory(), path);

				if (!Directory.Exists(fullPath))
				{
					Directory.CreateDirectory(fullPath);
				}

				var names = new CodegenNames(_controllerName);
				var options = default(CodegenOptions);

				if (_createArgs)
				{
					options |= CodegenOptions.CreateArgs;
				}

				if (_createCommands)
				{
					options |= CodegenOptions.CreateCommands;
				}

				try
				{
					// Controller
					File.WriteAllText(
						Path.Combine(fullPath, names.ControllerFileName),
						Codegen.GetControllerText(names, _namespace, _controllerBaseClass, GetPresentOptions(_exclusive, _popup, _modal), options));

					// Args
					if (_createArgs)
					{
						File.WriteAllText(
							Path.Combine(fullPath, names.ArgsFileName),
							Codegen.GetArgsText(names, _namespace, options));
					}

					// Commands
					if (_createCommands)
					{
						File.WriteAllText(
							Path.Combine(fullPath, names.CommandsFileName),
							Codegen.GetCommandsText(names, _namespace, options));
					}

					// View
					File.WriteAllText(
						Path.Combine(fullPath, names.ViewFileName),
						Codegen.GetViewText_UGUI(names, _namespace, _viewBaseClass, options));

					// View prefab.
					var go = new GameObject(_controllerName);
					var t = go.AddComponent<RectTransform>();

					try
					{
						go.layer = LayerMask.NameToLayer("UI");

						t.anchorMin = Vector2.zero;
						t.anchorMax = Vector2.one;
						t.anchoredPosition = Vector2.zero;
						t.sizeDelta = Vector2.zero;

						PrefabUtility.SaveAsPrefabAssetAndConnect(go, Path.Combine(path, names.PrefabFileName), InteractionMode.AutomatedAction);
					}
					finally
					{
						DestroyImmediate(go);
					}
				}
				finally
				{
					AssetDatabase.Refresh();
				}
			}
		}

		private static string GetPresentOptions(bool exclusive, bool popup, bool modal)
		{
			var text = string.Empty;

			if (exclusive || popup || modal)
			{
				text = string.Empty;

				if (exclusive)
				{
					text = nameof(PresentOptions) + '.' + nameof(PresentOptions.Exclusive);
				}

				if (popup)
				{
					if (!string.IsNullOrEmpty(text))
					{
						text += " | ";
					}

					text += nameof(PresentOptions) + '.' + nameof(PresentOptions.Popup);
				}

				if (modal)
				{
					if (!string.IsNullOrEmpty(text))
					{
						text += " | ";
					}

					text += nameof(PresentOptions) + '.' + nameof(PresentOptions.Modal);
				}
			}
			else
			{
				text = nameof(PresentOptions) + '.' + nameof(PresentOptions.None);
			}

			return text;
		}

		private static string GetSelectedPath()
		{
			var selection = Selection.assetGUIDs;

			if (selection != null && selection.Length == 1)
			{
				var path = AssetDatabase.GUIDToAssetPath(selection[0]);

				if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
				{
					return path;
				}
			}

			return null;
		}
	}
}
