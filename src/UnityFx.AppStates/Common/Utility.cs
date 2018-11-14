// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using System.Globalization;

namespace UnityFx.Mvc
{
	internal static class Utility
	{
		internal static string GetControllerTypeId(Type controllerType)
		{
			return GetDefaultControllerId(controllerType, false);
		}

		internal static string GetControllerDeeplinkId(Type controllerType)
		{
			return GetDefaultControllerId(controllerType, true);
		}

		private static string GetDefaultControllerId(Type controllerType, bool lowerCase)
		{
			var result = controllerType.Name.ToLowerInvariant();

			if (result.EndsWith("controller"))
			{
				if (lowerCase)
				{
					result = result.Substring(0, result.Length - 10);
				}
				else
				{
					result = controllerType.Name.Substring(0, result.Length - 10);
				}
			}

			return result;
		}
	}
}
