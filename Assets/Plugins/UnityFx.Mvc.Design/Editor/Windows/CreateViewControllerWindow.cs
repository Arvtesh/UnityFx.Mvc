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

				var indent = "	";
				var controllerName = _controllerName + "Controller";
				var controllerFileName = controllerName + ".cs";
				var controllerPath = Path.Combine(fullPath, controllerFileName);
				var controllerText = new StringBuilder();

				var argsName = _controllerName + "Args";
				var argsFileName = argsName + ".cs";
				var argsPath = Path.Combine(fullPath, argsFileName);
				var argsText = new StringBuilder();

				var commandsName = _controllerName + "Commands";
				var commandsFileName = commandsName + ".cs";
				var commandsPath = Path.Combine(fullPath, commandsFileName);
				var commandsText = new StringBuilder();

				var viewName = _controllerName + "View";
				var viewFileName = viewName + ".cs";
				var viewPath = Path.Combine(fullPath, viewFileName);
				var viewText = new StringBuilder();

				var prefabPath = Path.Combine(path, _controllerName + ".prefab");

				// Controller source.
				controllerText.AppendLine("using System;");
				controllerText.AppendLine("using System.Collections.Generic;");
				controllerText.AppendLine("using UnityEngine;");
				controllerText.AppendLine("using UnityFx.Mvc;");
				controllerText.AppendLine("");

				if (!string.IsNullOrEmpty(_namespace))
				{
					controllerText.AppendLine("namespace " + _namespace);
					controllerText.AppendLine("{");
				}
				else
				{
					indent = string.Empty;
				}

				controllerText.AppendLine(indent + "/// <summary>");
				controllerText.AppendLine(indent + "/// " + controllerName);
				controllerText.AppendLine(indent + "/// </summary>");
				controllerText.AppendFormat(indent + "/// <seealso cref=\"{0}\"/>" + Environment.NewLine, viewName);

				if (_createArgs)
				{
					controllerText.AppendFormat(indent + "/// <seealso cref=\"{0}\"/>" + Environment.NewLine, argsName);
				}

				if (_createCommands)
				{
					controllerText.AppendFormat(indent + "/// <seealso cref=\"{0}\"/>" + Environment.NewLine, commandsName);
				}

				controllerText.AppendFormat(indent + "[ViewController(PresentOptions = {0})]" + Environment.NewLine, GetPresentOptions(_exclusive, _popup, _modal));
				controllerText.AppendFormat(indent + "public class {0} : {1}<{2}>", controllerName, _controllerBaseClass, viewName);

				if (_createCommands)
				{
					controllerText.AppendFormat(", ICommandTarget<{0}>" + Environment.NewLine, commandsName);
				}

				controllerText.AppendLine(indent + "{");
				controllerText.AppendLine(indent + "	#region data");
				controllerText.AppendLine(indent + "	#endregion");
				controllerText.AppendLine("");
				controllerText.AppendLine(indent + "	#region interface");
				controllerText.AppendLine("");
				controllerText.AppendLine(indent + "	/// <summary>");
				controllerText.AppendFormat(indent + "	/// Initializes a new instance of the <see cref=\"{0}\"/> class." + Environment.NewLine, controllerName);
				controllerText.AppendLine(indent + "	/// </summary>");
				controllerText.AppendFormat(indent + "	public {0}({1} context", controllerName, nameof(IPresentContext));

				if (_createArgs)
				{
					controllerText.AppendFormat(", {0} args)" + Environment.NewLine, argsName);
				}
				else
				{
					controllerText.AppendLine(")");
				}

				controllerText.AppendLine(indent + "		: base(context)");
				controllerText.AppendLine(indent + "	{");
				controllerText.AppendLine(indent + "		// TODO: Initialize the controller view here. Add arguments to the Configure() as needed.");

				if (_createArgs)
				{
					controllerText.AppendLine(indent + "		View.Configure(args);");
				}
				else
				{
					controllerText.AppendLine(indent + "		View.Configure();");
				}

				controllerText.AppendLine(indent + "	}");
				controllerText.AppendLine("");
				controllerText.AppendLine(indent + "	#endregion");
				controllerText.AppendLine("");
				controllerText.AppendLine(indent + "	#region ViewController");
				controllerText.AppendLine("");
				controllerText.AppendLine(indent + "	/// <inheritdoc/>");
				controllerText.AppendLine(indent + "	protected override bool OnCommand(Command command, Variant args)");
				controllerText.AppendLine(indent + "	{");

				if (_createCommands)
				{
					controllerText.AppendLine(indent + $"		if (command.TryUnpackEnum(out {commandsName} cmd))");
					controllerText.AppendLine(indent + "		{");
					controllerText.AppendLine(indent + "			return InvokeCommand(cmd, args);");
					controllerText.AppendLine(indent + "		}");
					controllerText.AppendLine("");
				}
				else
				{
					controllerText.AppendLine(indent + "		// TODO: Process controller-specific commands here.");
				}

				controllerText.AppendLine(indent + "		return false;");
				controllerText.AppendLine(indent + "	}");
				controllerText.AppendLine("");
				controllerText.AppendLine(indent + "	#endregion");
				controllerText.AppendLine("");

				if (_createCommands)
				{
					controllerText.AppendLine(indent + "	#region ICommandTarget");
					controllerText.AppendLine("");
					controllerText.AppendLine(indent + "	/// <inheritdoc/>");
					controllerText.AppendLine(indent + $"	public bool InvokeCommand({commandsName} command, Variant args)");
					controllerText.AppendLine(indent + "	{");
					controllerText.AppendLine(indent + "		// TODO: Process constroller-specific commands here.");
					controllerText.AppendLine(indent + "		switch (command)");
					controllerText.AppendLine(indent + "		{");
					controllerText.AppendLine(indent + $"			case {commandsName}.Close:");
					controllerText.AppendLine(indent + "				Dismiss();");
					controllerText.AppendLine(indent + "				return true;");
					controllerText.AppendLine(indent + "		}");
					controllerText.AppendLine("");
					controllerText.AppendLine(indent + "		return false;");
					controllerText.AppendLine(indent + "	}");
					controllerText.AppendLine("");
					controllerText.AppendLine(indent + "	#endregion");
					controllerText.AppendLine("");
				}

				controllerText.AppendLine(indent + "	#region implementation");
				controllerText.AppendLine(indent + "	#endregion");
				controllerText.AppendLine(indent + "}");

				if (!string.IsNullOrEmpty(_namespace))
				{
					controllerText.AppendLine("}");
				}

				File.WriteAllText(controllerPath, controllerText.ToString());

				// View source.
				viewText.AppendLine("using System;");
				viewText.AppendLine("using System.Collections.Generic;");
				viewText.AppendLine("using UnityEngine;");
				viewText.AppendLine("using UnityEngine.UI;");
				viewText.AppendLine("using UnityFx.Mvc;");
				viewText.AppendLine("");

				if (!string.IsNullOrEmpty(_namespace))
				{
					viewText.AppendLine("namespace " + _namespace);
					viewText.AppendLine("{");
				}

				viewText.AppendLine(indent + "/// <summary>");
				viewText.AppendFormat(indent + "/// View for the <see cref=\"{0}\"/>." + Environment.NewLine, controllerName);
				viewText.AppendLine(indent + "/// </summary>");
				viewText.AppendFormat(indent + "/// <seealso cref=\"{0}\"/>" + Environment.NewLine, controllerName);

				if (_createArgs)
				{
					viewText.AppendFormat(indent + "/// <seealso cref=\"{0}\"/>" + Environment.NewLine, argsName);
				}

				if (_createCommands)
				{
					viewText.AppendFormat(indent + "/// <seealso cref=\"{0}\"/>" + Environment.NewLine, commandsName);
				}

				viewText.AppendFormat(indent + "public class {0} : {1}" + Environment.NewLine, viewName, _viewBaseClass);
				viewText.AppendLine(indent + "{");
				viewText.AppendLine(indent + "	#region data");
				viewText.AppendLine("");
				viewText.AppendLine("#pragma warning disable 0649");
				viewText.AppendLine("");
				viewText.AppendLine(indent + "	// TODO: Add serializable fields here.");
				viewText.AppendLine("");
				viewText.AppendLine("#pragma warning restore 0649");
				viewText.AppendLine("");
				viewText.AppendLine(indent + "	#endregion");
				viewText.AppendLine("");
				viewText.AppendLine(indent + "	#region interface");
				viewText.AppendLine("");
				viewText.AppendLine(indent + "	/// <summary>");
				viewText.AppendLine(indent + "	/// Initializes the view. Called from the controller ctor." );
				viewText.AppendLine(indent + "	/// </summary>");

				if (_createArgs)
				{
					viewText.AppendFormat(indent + "	public void Configure({0} args)" + Environment.NewLine, argsName);
				}
				else
				{
					viewText.AppendLine(indent + "	public void Configure()");
				}

				viewText.AppendLine(indent + "	{");
				viewText.AppendLine(indent + "		// TODO: Setup the view. Add additional arguments as needed.");
				viewText.AppendLine(indent + "	}");
				viewText.AppendLine("");
				viewText.AppendLine(indent + "	#endregion");
				viewText.AppendLine("");
				viewText.AppendLine(indent + "	#region MonoBehaviour");
				viewText.AppendLine(indent + "	#endregion");
				viewText.AppendLine("");
				viewText.AppendLine(indent + "	#region implementation");
				viewText.AppendLine(indent + "	#endregion");
				viewText.AppendLine(indent + "}");

				if (!string.IsNullOrEmpty(_namespace))
				{
					viewText.AppendLine("}");
				}

				File.WriteAllText(viewPath, viewText.ToString());

				// Args source.
				if (_createArgs)
				{
					argsText.AppendLine("using System;");
					argsText.AppendLine("using UnityEngine;");
					argsText.AppendLine("using UnityFx.Mvc;");
					argsText.AppendLine("");

					if (!string.IsNullOrEmpty(_namespace))
					{
						argsText.AppendLine("namespace " + _namespace);
						argsText.AppendLine("{");
					}

					argsText.AppendLine(indent + "/// <summary>");
					argsText.AppendFormat(indent + "/// Arguments for the <see cref=\"{0}\"/>." + Environment.NewLine, controllerName);
					argsText.AppendLine(indent + "/// </summary>");
					argsText.AppendFormat(indent + "/// <seealso cref=\"{0}\"/>" + Environment.NewLine, controllerName);
					argsText.AppendFormat(indent + "/// <seealso cref=\"{0}\"/>" + Environment.NewLine, viewName);

					argsText.AppendFormat(indent + "public class {0} : PresentArgs" + Environment.NewLine, argsName);
					argsText.AppendLine(indent + "{");
					argsText.AppendLine(indent + "}");

					if (!string.IsNullOrEmpty(_namespace))
					{
						argsText.AppendLine("}");
					}

					File.WriteAllText(argsPath, argsText.ToString());
				}

				// Commands source.
				if (_createCommands)
				{
					commandsText.AppendLine("using System;");
					commandsText.AppendLine("");

					if (!string.IsNullOrEmpty(_namespace))
					{
						commandsText.AppendLine("namespace " + _namespace);
						commandsText.AppendLine("{");
					}

					commandsText.AppendLine(indent + "	/// <summary>");
					commandsText.AppendLine(indent + $"	/// Enumerates <see cref=\"{controllerName}\"/>-specific commands.");
					commandsText.AppendLine(indent + "	/// </summary>");
					commandsText.AppendLine(indent + $"	/// <seealso cref=\"{controllerName}\"/>");
					commandsText.AppendLine(indent + $"	/// <seealso cref=\"{viewName}\"/>");
					commandsText.AppendLine(indent + $"	public enum {commandsName}");
					commandsText.AppendLine(indent + "	{");
					commandsText.AppendLine(indent + "		Close");
					commandsText.AppendLine(indent + "	}");
					commandsText.AppendLine("");

					if (!string.IsNullOrEmpty(_namespace))
					{
						commandsText.AppendLine("}");
					}

					File.WriteAllText(commandsPath, commandsText.ToString());
				}

				AssetDatabase.Refresh();

				// Prefab.
				var go = new GameObject(_controllerName);
				var t = go.AddComponent<RectTransform>();

				try
				{
					t.anchorMin = Vector2.zero;
					t.anchorMax = Vector2.one;
					t.anchoredPosition = Vector2.zero;
					t.sizeDelta = Vector2.zero;

					PrefabUtility.SaveAsPrefabAssetAndConnect(go, prefabPath, InteractionMode.AutomatedAction);
				}
				finally
				{
					DestroyImmediate(go);
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
