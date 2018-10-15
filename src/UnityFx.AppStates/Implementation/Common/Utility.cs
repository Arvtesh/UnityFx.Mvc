// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
#if !NET35
using System.Runtime.ExceptionServices;
#endif

namespace UnityFx.AppStates
{
	internal static class Utility
	{
		internal static object CreateInstance(IServiceProvider serviceProvider, Type serviceType, params object[] args)
		{
			Debug.Assert(serviceProvider != null);
			Debug.Assert(serviceType != null);

			try
			{
				var constructors = serviceType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

				if (constructors.Length > 0)
				{
					// Select the first public non-static ctor with matching arguments.
					foreach (var ctor in constructors)
					{
						if (TryGetMethodArguments(ctor, serviceProvider, args, out var argValues))
						{
							return ctor.Invoke(argValues);
						}
					}

					throw new InvalidOperationException($"A suitable constructor for type '{serviceType}' could not be located. Ensure the type is concrete and services are registered for all parameters of a public constructor.");
				}
				else
				{
					return Activator.CreateInstance(serviceType);
				}
			}
			catch (TargetInvocationException e)
			{
#if !NET35
				ExceptionDispatchInfo.Capture(e.InnerException).Throw();
#endif
				throw e.InnerException;
			}
		}

		internal static string GetControllerTypeId(Type controllerType)
		{
			return GetDefaultControllerId(controllerType, false);
		}

		internal static string GetControllerDeeplinkId(Type controllerType)
		{
			return GetDefaultControllerId(controllerType, true);
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
				result = GetDefaultControllerId(controllerType, false);
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

		private static bool TryGetMethodArguments(MethodBase method, IServiceProvider serviceProvider, object[] args, out object[] argValues)
		{
			var argInfo = method.GetParameters();
			var argumentsValidated = true;

			argValues = new object[argInfo.Length];

			for (var i = 0; i < argInfo.Length; ++i)
			{
				var argType = argInfo[i].ParameterType;
				var argValue = default(object);

				// Try to match the argument using args first.
				for (var j = 0; j < args.Length; ++j)
				{
					if (argType.IsAssignableFrom(args[j].GetType()))
					{
						argValue = args[j];
						break;
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
