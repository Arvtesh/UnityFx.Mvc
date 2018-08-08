// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using System.Globalization;

namespace UnityFx.AppStates
{
	internal static class Utility
	{
		internal static void InvokePresentableOnDismiss(IPresentable controller)
		{
			if (controller is IPresentableEvents pe)
			{
				pe.OnDismiss();
			}
		}

		internal static string GetPresentableTypeId(Type controllerType)
		{
			string result = null;

			if (Attribute.GetCustomAttribute(controllerType, typeof(AppViewControllerAttribute)) is AppViewControllerAttribute attr)
			{
				if (!string.IsNullOrEmpty(attr.Id))
				{
					result = attr.Id;
				}
			}

			if (string.IsNullOrEmpty(result))
			{
				result = controllerType.Name.ToLowerInvariant();

				if (result.EndsWith("controller"))
				{
					result = result.Substring(0, result.Length - 10);
				}
			}

			return result;
		}

		internal static PresentOptions GetPresentableOptions(Type controllerType)
		{
			if (Attribute.GetCustomAttribute(controllerType, typeof(AppViewControllerAttribute)) is AppViewControllerAttribute attr)
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
	}
}
