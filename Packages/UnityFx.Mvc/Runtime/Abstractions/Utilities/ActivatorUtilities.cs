// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// <see cref="Activator"/>-related utilities.
	/// </summary>
	public static class ActivatorUtilities
	{
		/// <summary>
		/// Instantiates a prefab making sure it has a component of type <typeparamref name="TComponent"/> attached.
		/// </summary>
		public static TComponent InstantiatePrefab<TComponent>(GameObject prefab, Transform parent) where TComponent : Component
		{
			if (prefab is null)
			{
				throw new ArgumentNullException(nameof(prefab));
			}

			var go = GameObject.Instantiate(prefab, parent, false);
			var view = go.GetComponent<TComponent>();

			if (view is null)
			{
				view = go.AddComponent<TComponent>();
			}

			return view;
		}

		/// <summary>
		/// Instantiates a view prefab making sure it has a view attached.
		/// </summary>
		public static TView InstantiateViewPrefab<TView>(GameObject prefab, Transform parent) where TView : Component, IView
		{
			return InstantiatePrefab<TView>(prefab, parent);
		}

		/// <summary>
		/// Creates an instance of the specified type using values from <paramref name="serviceProvider"/> and <paramref name="args"/> as arguments.
		/// Values from <paramref name="args"/> have priority over values retrieved from <paramref name="serviceProvider"/>.
		/// </summary>
		/// <typeparam name="T">The type to instantiate.</typeparam>
		/// <param name="serviceProvider">A service provider to get argument values from.</param>
		/// <param name="args">Agument values to use.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceProvider"/> is <see langword="null"/>.</exception>
		/// <returns>Returns the instance created.</returns>
		public static T CreateInstance<T>(IServiceProvider serviceProvider, object[] args)
		{
			return (T)CreateInstance(typeof(T), serviceProvider, args);
		}

		/// <summary>
		/// Creates an instance of the specified <paramref name="type"/> using values from <paramref name="serviceProvider"/> and <paramref name="args"/> as arguments.
		/// Values from <paramref name="args"/> have priority over values retrieved from <paramref name="serviceProvider"/>.
		/// </summary>
		/// <param name="type">Type to instantiate.</param>
		/// <param name="serviceProvider">A service provider to get argument values from.</param>
		/// <param name="args">Agument values to use.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="type"/> or <paramref name="serviceProvider"/> is <see langword="null"/>.</exception>
		/// <returns>Returns the instance created.</returns>
		public static object CreateInstance(Type type, IServiceProvider serviceProvider, object[] args)
		{
			if (type is null)
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
				ExceptionDispatchInfo.Capture(e.InnerException).Throw();
				throw e.InnerException;
			}
		}

		/// <summary>
		/// Attempts to collect arguments for <paramref name="method"/> using the specified <paramref name="serviceProvider"/> and <paramref name="args"/>.
		/// Values from <paramref name="args"/> have priority over values retrieved from <paramref name="serviceProvider"/>.
		/// </summary>
		/// <param name="method">The method to get argument values for.</param>
		/// <param name="serviceProvider">A service provider to get argument values from.</param>
		/// <param name="args">Agument values to use.</param>
		/// <param name="argValues">Resulting array of argument values.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="method"/> or <paramref name="serviceProvider"/> is <see langword="null"/>.</exception>
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
				if (argValue is null)
				{
					argValue = serviceProvider.GetService(argType);
				}

				// If the argument is matched/resolved, store the value, otherwise fail the constructor validation.
				if (argValue is null)
				{
					argumentsValidated = false;
					break;
				}
				else
				{
					argValues[i] = argValue;
				}
			}

			return argumentsValidated;
		}
	}
}
