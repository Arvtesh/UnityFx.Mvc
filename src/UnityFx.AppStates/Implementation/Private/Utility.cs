// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using System.Globalization;

namespace UnityFx.AppStates
{
	internal static class Utility
	{
		internal static string GetControllerTypeId(Type controllerType)
		{
			return GetDefaultControllerId(controllerType);
		}

		internal static string GetViewResourceId(Type controllerType)
		{
			string result = null;

			if (Attribute.GetCustomAttribute(controllerType, typeof(ViewResourceAttribute)) is ViewResourceAttribute attr)
			{
				if (!string.IsNullOrEmpty(attr.ResourceId))
				{
					result = attr.ResourceId;
				}
			}

			if (string.IsNullOrEmpty(result))
			{
				result = GetDefaultControllerId(controllerType);
			}

			return result;
		}

		internal static PresentOptions GetControllerOptions(Type controllerType)
		{
			if (Attribute.GetCustomAttribute(controllerType, typeof(ViewControllerOptionsAttribute)) is ViewControllerOptionsAttribute attr)
			{
				return attr.Options;
			}

			return PresentOptions.None;
		}

		internal static string GetNextId(string typeId, ref int counter)
		{
			var id = ++counter;

			if (id <= 0)
			{
				id = 1;
			}

			return typeId + id.ToString(CultureInfo.InvariantCulture);
		}

		private static string GetDefaultControllerId(Type controllerType)
		{
			var result = controllerType.Name.ToLowerInvariant();

			if (result.EndsWith("controller"))
			{
				result = result.Substring(0, result.Length - 10);
			}

			return result;
		}
	}
}
