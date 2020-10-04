// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityFx.Mvc
{
	internal static class Codegen
	{
		private const string _indent = "\t";
		private const string _inheritdoc = "/// <inheritdoc/>";
		private const string _summaryBegin = "/// <summary>";
		private const string _summary = "/// ";
		private const string _summaryEnd = "/// <summary>";
		private const string _seeAlso = "/// <seealso cref=\"{0}\"/>";

		private static readonly Regex _classNamePattern = new Regex("^[_a-zA-Z][_a-zA-Z0-9]*$");

		public static string GetControllerText(CodegenNames names, string namespaceName, string baseClassName, string flags, CodegenOptions options)
		{
			var sb = new StringBuilder(256);
			var text = new TextHelper(sb);

			text.AppendLine("using System;");
			text.AppendLine("using System.Collections.Generic;");
			text.AppendLine("using System.Threading.Tasks;");
			text.AppendLine("using UnityEngine;");
			text.AppendLine("using UnityFx.Mvc;");
			text.AppendLine();

			using (var namespaceScope = new NamespaceScope(sb, namespaceName))
			{
				text.Indent = namespaceScope.Ident;
				text.AppendSummmary(names.ControllerName);
				text.AppendSeeAlso(names.ViewName);

				if (options.HasFlag(CodegenOptions.CreateArgs))
				{
					text.AppendSeeAlso(names.ArgsName);
				}

				if (options.HasFlag(CodegenOptions.CreateCommands))
				{
					text.AppendSeeAlso(names.CommandsName);
				}

				//if (!string.IsNullOrEmpty(flags))
				//{
				//	text.AppendLineFormat("[ViewController({0} = {1})]", nameof(ViewControllerAttribute.Flags), flags);
				//}

				if (options.HasFlag(CodegenOptions.CreateCommands))
				{
					text.AppendLineFormat("public class {0} : {1}<{2}>, {3}<{4}>", names.ControllerName, baseClassName, names.ViewName, nameof(ICommandTarget), names.CommandsName);
				}
				else
				{
					text.AppendLineFormat("public class {0} : {1}<{2}>", names.ControllerName, baseClassName, names.ViewName);
				}

				text.AppendLine("{");
				text.Indent += 1;
				{
					// #region data
					text.AppendLine("#region data");
					text.AppendLine("#endregion");
					text.AppendLine();

					// #region interface
					text.AppendLine("#region interface");
					text.AppendLine();

					// ctor
					text.AppendSummmary("Initializes a new instance of the <see cref=\"{0}\"/> class.", names.ControllerName);

					if (options.HasFlag(CodegenOptions.CreateArgs))
					{
						text.AppendLineFormat("public {0}({1} context, {2} args)", names.ControllerName, nameof(IPresentContext), names.ArgsName);
					}
					else
					{
						text.AppendLineFormat("public {0}({1} context)", names.ControllerName, nameof(IPresentContext));
					}

					text.AppendLine("	: base(context)");
					text.AppendLine("{");
					text.Indent += 1;
					{
						text.AppendLine("// TODO: Initialize the controller view here. Add arguments to the Configure() as needed.");

						if (options.HasFlag(CodegenOptions.CreateArgs))
						{
							text.AppendLine("View.Configure(args);");
						}
						else
						{
							text.AppendLine("View.Configure();");
						}
					}
					text.Indent -= 1;
					text.AppendLine("}");
					text.AppendLine();
					text.AppendLine("#endregion");
					text.AppendLine();

					// #region ViewController
					text.AppendLineFormat("#region {0}", nameof(ViewController));
					text.AppendLine();

					// OnCommand()
					text.AppendLine(_inheritdoc);
					text.AppendLineFormat("protected override bool OnCommand({0} command, {1} args)", nameof(Command), nameof(Variant));
					text.AppendLine("{");
					text.Indent += 1;
					{
						if (options.HasFlag(CodegenOptions.CreateCommands))
						{
							text.AppendLineFormat("if (command.{0}(out {1} cmd))", nameof(Command.TryParse), names.CommandsName);
							text.AppendLine("{");
							text.Indent += 1;
							{
								text.AppendLine("return InvokeCommand(cmd, args);");
							}
							text.Indent -= 1;
							text.AppendLine("}");
							text.AppendLine();
						}
						else
						{
							text.AppendLine("// TODO: Process controller-specific commands here.");
						}

						text.AppendLine("return false;");
					}
					text.Indent -= 1;
					text.AppendLine("}");

					text.AppendLine();
					text.AppendLine("#endregion");
					text.AppendLine();

					if (options.HasFlag(CodegenOptions.CreateCommands))
					{
						// #region ICommandTarget
						text.AppendLineFormat("#region {0}", nameof(ICommandTarget));
						text.AppendLine();

						// InvokeCommand()
						text.AppendLine(_inheritdoc);
						text.AppendLineFormat("public bool {0}({1} command, {2} args)", nameof(ICommandTarget.InvokeCommand), names.CommandsName, nameof(Variant));
						text.AppendLine("{");
						text.Indent += 1;
						{
							text.AppendLine("// TODO: Process constroller-specific commands here.");
							text.AppendLine("switch (command)");
							text.AppendLine("{");
							text.Indent += 1;
							{
								text.AppendLineFormat("case {0}.Close:", names.CommandsName);
								text.Indent += 1;
								{
									text.AppendLine("Dismiss();");
									text.AppendLine("return true;");
								}
								text.Indent -= 1;
							}
							text.Indent -= 1;
							text.AppendLine("}");
							text.AppendLine();
							text.AppendLine("return false;");
						}
						text.Indent -= 1;
						text.AppendLine("}");
						text.AppendLine();
						text.AppendLine("#endregion");
						text.AppendLine();
					}

					// #region implementation
					text.AppendLine("#region implementation");
					text.AppendLine("#endregion");
				}
				text.Indent -= 1;
				text.AppendLine("}");
			}

			return text.ToString();
		}

		public static string GetViewText(CodegenNames names, string namespaceName, string baseClassName, CodegenOptions options)
		{
			var sb = new StringBuilder(256);
			var text = new TextHelper(sb);

			text.AppendLine("using System;");
			text.AppendLine("using UnityEngine;");
			text.AppendLine("using UnityFx.Mvc;");
			text.AppendLine();

			using (var namespaceScope = new NamespaceScope(sb, namespaceName))
			{
				text.Indent = namespaceScope.Ident;
				text.AppendSummmary("View for the <see cref=\"{0}\"/>.", names.ControllerName);
				text.AppendSeeAlso(names.ControllerName);

				if (options.HasFlag(CodegenOptions.CreateArgs))
				{
					text.AppendSeeAlso(names.ArgsName);
				}

				if (options.HasFlag(CodegenOptions.CreateCommands))
				{
					text.AppendSeeAlso(names.CommandsName);
				}

				text.AppendLineFormat("public class {0} : {1}", names.ViewName, baseClassName);
				text.AppendLine("{");
				text.Indent += 1;
				{
					// #region data
					text.AppendLine("#region data");
					text.AppendLine();
					text.AppendLine("#pragma warning disable 0649");
					text.AppendLine();
					text.AppendLine("// TODO: Add serializable fields here.");
					text.AppendLine();
					text.AppendLine("#pragma warning restore 0649");
					text.AppendLine();
					text.AppendLine("#endregion");
					text.AppendLine();

					// #region interface
					text.AppendLine("#region interface");
					text.AppendLine();

					// Configure()
					text.AppendSummmary("Initializes the view. Called from the controller ctor.");

					if (options.HasFlag(CodegenOptions.CreateArgs))
					{
						text.AppendLineFormat("public void Configure({0} args)", names.ArgsName);
					}
					else
					{
						text.AppendLine("public void Configure()");
					}

					text.AppendLine("{");
					text.Indent += 1;
					{
						text.AppendLine("// TODO: Setup the view here. Add additional arguments as needed.");
					}
					text.Indent -= 1;
					text.AppendLine("}");
					text.AppendLine();
					text.AppendLine("#endregion");
					text.AppendLine();

					// #region MonoBehaviour
					text.AppendLine("#region MonoBehaviour");
					text.AppendLine("#endregion");
					text.AppendLine();

					// #region implementation
					text.AppendLine("#region implementation");
					text.AppendLine("#endregion");
				}
				text.Indent -= 1;
				text.AppendLine("}");
			}

			return text.ToString();
		}

		public static string GetArgsText(CodegenNames names, string namespaceName, CodegenOptions options)
		{
			var sb = new StringBuilder(256);
			var text = new TextHelper(sb);

			text.AppendLine("using System;");
			text.AppendLine("using UnityEngine;");
			text.AppendLine("using UnityFx.Mvc;");
			text.AppendLine();

			using (var namespaceScope = new NamespaceScope(sb, namespaceName))
			{
				text.Indent = namespaceScope.Ident;
				text.AppendSummmary("Arguments for the <see cref=\"{0}\"/>.", names.ControllerName);
				text.AppendSeeAlso(names.ControllerName);
				text.AppendSeeAlso(names.ViewName);

				if (options.HasFlag(CodegenOptions.CreateCommands))
				{
					text.AppendSeeAlso(names.CommandsName);
				}

				text.AppendLineFormat("public class {0} : {1}", names.ArgsName, nameof(PresentArgs));
				text.AppendLine("{");
				text.AppendLine("}");
			}

			return text.ToString();
		}

		public static string GetCommandsText(CodegenNames names, string namespaceName, CodegenOptions options)
		{
			var sb = new StringBuilder(256);
			var text = new TextHelper(sb);

			text.AppendLine("using System;");
			text.AppendLine();

			using (var namespaceScope = new NamespaceScope(sb, namespaceName))
			{
				text.Indent = namespaceScope.Ident;
				text.AppendSummmary("Enumerates <see cref=\"{0}\"/>-specific commands.", names.ControllerName);
				text.AppendSeeAlso(names.ControllerName);
				text.AppendSeeAlso(names.ViewName);
				text.AppendLineFormat("public enum {0}", names.CommandsName);
				text.AppendLine("{");
				text.Indent += 1;
				text.AppendLine("Close");
				text.Indent -= 1;
				text.AppendLine("}");
			}

			return text.ToString();
		}

		private struct TextHelper
		{
			private readonly StringBuilder _text;
			private int _indentNumber;

			public int Indent { get => _indentNumber; set => _indentNumber = value; }

			public TextHelper(StringBuilder text)
			{
				_text = text;
				_indentNumber = 0;
			}

			public void AppendLine(string s)
			{
				AppendIndent(_text, _indentNumber);
				_text.AppendLine(s);
			}

			public void AppendLineFormat(string f, params string[] args)
			{
				AppendIndent(_text, _indentNumber);
				_text.AppendFormat(f, args);
				_text.AppendLine();
			}

			public void AppendLine()
			{
				_text.AppendLine();
			}

			public void AppendSummmary(string f, params string[] args)
			{
				AppendIndent(_text, _indentNumber);
				_text.AppendLine(_summaryBegin);

				AppendIndent(_text, _indentNumber);
				_text.Append(_summary);
				_text.AppendFormat(f, args);
				_text.AppendLine();

				AppendIndent(_text, _indentNumber);
				_text.AppendLine(_summaryEnd);
			}

			public void AppendSeeAlso(string name)
			{
				AppendIndent(_text, _indentNumber);
				_text.AppendFormat(_seeAlso, name);
				_text.AppendLine();
			}

			public override string ToString()
			{
				return _text.ToString();
			}

			private static void AppendIndent(StringBuilder text, int indent)
			{
				while (indent-- > 0)
				{
					text.Append(_indent);
				}
			}
		}

		private readonly struct NamespaceScope : IDisposable
		{
			private readonly StringBuilder _text;
			private readonly int _indentValue;

			public int Ident => _indentValue;

			public NamespaceScope(StringBuilder text, string namespaceName)
			{
				_text = text;

				if (!string.IsNullOrEmpty(namespaceName))
				{
					_indentValue = 1;

					text.AppendLine("namespace " + namespaceName);
					text.AppendLine("{");
				}
				else
				{
					_indentValue = 0;
				}
			}

			public void Dispose()
			{
				if (_indentValue > 0)
				{
					_text.AppendLine("}");
				}
			}
		}
	}
}
