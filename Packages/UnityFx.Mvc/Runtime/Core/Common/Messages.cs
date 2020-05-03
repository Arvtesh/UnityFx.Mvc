// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	internal static class Messages
	{
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

		public static string Format_InvalidViewType(Type expectedType)
		{
			return $"Invalid view type, {expectedType.Name} is expected. Make sure view prefab has the correct view attached.";
		}
	}
}
