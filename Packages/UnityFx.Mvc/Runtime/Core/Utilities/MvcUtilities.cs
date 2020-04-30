// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Reflection helpers.
	/// </summary>
	public static class MvcUtilities
	{
		/// <summary>
		/// TODO
		/// </summary>
		public static TView InstantiateViewPrefab<TView>(GameObject prefab, Transform parent, string resourceId) where TView : View
		{
			if (prefab is null)
			{
				throw new ArgumentNullException(nameof(prefab));
			}

			var go = GameObject.Instantiate(prefab, parent, false);
			var view = go.GetComponent<TView>();

			if (view is null)
			{
				view = go.AddComponent<TView>();
			}

			view.SetResourceId(resourceId);
			return view;
		}

		/// <summary>
		/// TODO
		/// </summary>
		public static string GetControllerDeeplinkId(Type controllerType)
		{
			var deeplinkId = controllerType.Name.ToLower();

			if (deeplinkId.EndsWith("controller"))
			{
				deeplinkId = deeplinkId.Substring(0, deeplinkId.Length - 10);
			}

			return deeplinkId;
		}

		/// <summary>
		/// TODO
		/// </summary>
		public static string GetControllerName(Type controllerType)
		{
			return GetControllerName(controllerType.Name);
		}

		/// <summary>
		/// TODO
		/// </summary>
		public static string GetControllerName(string controllerTypeName)
		{
			if (controllerTypeName.EndsWith("Controller"))
			{
				return controllerTypeName.Substring(0, controllerTypeName.Length - 10);
			}

			return controllerTypeName;
		}

		/// <summary>
		/// TODO
		/// </summary>
		public static bool IsAssignableToGenericType(Type givenType, Type genericType, out Type closedGenericType)
		{
			// NOTE: See https://stackoverflow.com/questions/5461295/using-isassignablefrom-with-open-generic-types for details.
			var interfaceTypes = givenType.GetInterfaces();

			foreach (var it in interfaceTypes)
			{
				if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
				{
					closedGenericType = it;
					return true;
				}
			}

			if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
			{
				closedGenericType = givenType;
				return true;
			}

			var baseType = givenType.BaseType;

			if (baseType == null)
			{
				closedGenericType = null;
				return false;
			}

			return IsAssignableToGenericType(baseType, genericType, out closedGenericType);
		}
	}
}
