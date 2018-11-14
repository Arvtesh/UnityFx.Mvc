// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
#if !NET35
using System.Runtime.ExceptionServices;
#endif

namespace UnityFx.Mvc
{
	/// <summary>
	/// Defines <see cref="Activator"/>-related helpers.
	/// </summary>
	public static class ActivatorUtilities
	{
		#region interface

		/// <summary>
		/// Creates an object of a specific type matching constructor arguments with values of <paramref name="args"/> and <paramref name="serviceProvider"/> specified.
		/// </summary>
		/// <param name="serviceProvider">A service provider used to match <paramref name="type"/> constructor arguments.</param>
		/// <param name="type">A type to instantiate.</param>
		/// <param name="args">An optional list of the <paramref name="type"/> constructor argument values.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> or <paramref name="serviceProvider"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the <paramref name="type"/> cannot be instantiated.</exception>
		/// <returns>A <paramref name="type"/> instance created.</returns>
		public static object CreateInstance(IServiceProvider serviceProvider, Type type, params object[] args)
		{
			if (serviceProvider == null)
			{
				throw new ArgumentNullException(nameof(serviceProvider));
			}

			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			try
			{
				var constructors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

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

					throw new InvalidOperationException($"A suitable constructor for type '{type}' could not be located. Ensure the type is concrete and services are registered for all parameters of a public constructor.");
				}
				else
				{
					return Activator.CreateInstance(type);
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

		#endregion

		#region implementation

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
					var arg = args[j];

					if (arg != null && argType.IsAssignableFrom(arg.GetType()))
					{
						argValue = arg;
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

		#endregion
	}
}
