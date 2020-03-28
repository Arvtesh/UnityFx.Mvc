// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	internal readonly struct CodegenNames
	{
		private readonly string _baseName;

		public string BaseName => _baseName;
		public string ControllerName => _baseName + "Controller";
		public string ControllerFileName => _baseName + "Controller.cs";
		public string ViewName => _baseName + "View";
		public string ViewFileName => _baseName + "View.cs";
		public string ArgsName => _baseName + "Args";
		public string ArgsFileName => _baseName + "Args.cs";
		public string CommandsName => _baseName + "Commands";
		public string CommandsFileName => _baseName + "Commands.cs";
		public string PrefabFileName => _baseName + ".prefab";

		public CodegenNames(string name)
		{
			_baseName = name;
		}
	}
}
