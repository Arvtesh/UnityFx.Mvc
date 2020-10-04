// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	internal static class Messages
	{
		public static string Format_ControllerAlreadyPresented(Type controllerType)
		{
			return $"Controller of the same type is already presented. Use {ViewControllerFlags.AllowMultipleInstances} flag to allow this behavior.";
		}

		public static string Format_ControllerTypeIsAbstract(Type controllerType)
		{
			return $"Cannot instantiate abstract type {controllerType.Name}.";
		}

		public static string Format_ControllerTypeIsNotController(Type controllerType)
		{
			return $"A view controller is expected to implement {typeof(IViewController).Name}.";
		}

		public static string Format_InvalidPrefabPath()
		{
			return "Invalid prefab path.";
		}

		public static string Format_PrefabIsNull()
		{
			return "Invalid is null.";
		}

		public static string Format_PrefabCannotBeLoaded(string prefabPath)
		{
			return $"Prefab '{prefabPath}' is not preloaded and no load delegate is set.";
		}

		public static string Format_ViewIsNull()
		{
			return "View is null.";
		}

		public static string Format_InvalidArgsType(Type expectedType)
		{
			return $"Invalid arguments type, {expectedType.Name} is expected.";
		}

		public static string Format_InvalidViewType(Type expectedType)
		{
			return $"Invalid view type, {expectedType.Name} is expected. Make sure view prefab has the correct view attached.";
		}
	}
}
