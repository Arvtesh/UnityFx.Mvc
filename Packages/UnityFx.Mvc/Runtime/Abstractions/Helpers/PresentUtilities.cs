// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Reflection;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Present utilities.
	/// </summary>
	public static class PresentUtilities
	{
		/// <summary>
		/// Instantiates a view prefab making sure it has a view attached.
		/// </summary>
		public static TView InstantiateViewPrefab<TView>(GameObject prefab, Transform parent) where TView : Component, IView
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

			return view;
		}

		/// <summary>
		/// Gets default deeplink identifier for the controller type specified.
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
		/// Gets default controller identifier for the type specified.
		/// </summary>
		public static string GetControllerName(Type controllerType)
		{
			return GetControllerName(controllerType.Name);
		}

		/// <summary>
		/// Gets default controller identifier for the type name specified.
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
		/// Checks if <paramref name="genericType"/> is assignable to <paramref name="type"/> and, if it is, returnes closed generic type.
		/// </summary>
		/// <seealso href="https://stackoverflow.com/questions/5461295/using-isassignablefrom-with-open-generic-types"/>
		public static bool IsAssignableToGenericType(Type type, Type genericType, out Type resultType)
		{
			foreach (var it in type.GetInterfaces())
			{
				if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
				{
					resultType = it;
					return true;
				}
			}

			if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType)
			{
				resultType = type;
				return true;
			}

			var baseType = type.BaseType;

			if (baseType == null)
			{
				resultType = null;
				return false;
			}

			return IsAssignableToGenericType(baseType, genericType, out resultType);
		}

		/// <summary>
		/// Attempts to collect arguments for <paramref name="method"/> using the specified <paramref name="serviceProvider"/> and <paramref name="args"/> passed.
		/// Values from <paramref name="args"/> have priority over values retrieved from <paramref name="serviceProvider"/>.
		/// </summary>
		/// <param name="method">The method to get argument values for.</param>
		/// <param name="serviceProvider">A service provider to get argument values from.</param>
		/// <param name="args">Agument values to use.</param>
		/// <param name="argValues">Resulting array of argument values.</param>
		/// <returns>Returns <see langword="true"/> if values were found for all <paramref name="method"/> argumentsl <see langword="false"/> otherwise.</returns>
		public static bool TryGetMethodArguments(MethodBase method, IServiceProvider serviceProvider, object[] args, out object[] argValues)
		{
			if (method is null)
			{
				throw new ArgumentNullException(nameof(method));
			}

			if (serviceProvider is null)
			{
				throw new ArgumentNullException(nameof(serviceProvider));
			}

			var argInfo = method.GetParameters();
			var argumentsValidated = true;

			argValues = new object[argInfo.Length];

			for (var i = 0; i < argInfo.Length; ++i)
			{
				var argType = argInfo[i].ParameterType;
				var argValue = default(object);

				// Try to match the argument using args first.
				if (args != null)
				{
					for (var j = 0; j < args.Length; ++j)
					{
						var arg = args[j];

						if (arg != null && argType.IsAssignableFrom(arg.GetType()))
						{
							argValue = arg;
							break;
						}
					}
				}

				// If argument matching failed try to resolve the argument using serviceProvider.
				if (argValue == null)
				{
					argValue = serviceProvider.GetService(argType);
				}

				// If the argument is matched/resolved, store the value, otherwise fail the constructor validation.
				if (argValue != null)
				{
					argValues[i] = argValue;
				}
				else
				{
					argumentsValidated = false;
					break;
				}
			}

			// If all arguments matched/resolved, use this constructor for activation.
			if (argumentsValidated)
			{
				return true;
			}

			return false;
		}
	}
}
