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
		/// Name of the initialize method.
		/// </summary>
		public const string InitializeMethodName = "Initialize";

		/// <summary>
		/// Name of the configure method.
		/// </summary>
		public const string ConfigureMethodName = "Configure";

		/// <summary>
		/// Attempts to configure an object, i.e. call <see cref="IConfigurable{T}.Configure(T)"/> method on the <paramref name="obj"/>.
		/// </summary>
		/// <seealso cref="IConfigurable{T}"/>
		public static bool TryConfigure(object obj, object args)
		{
			if (obj != null && args != null)
			{
				var configurableType = typeof(IConfigurable<>).MakeGenericType(args.GetType());
				var viewType = obj.GetType();

				if (configurableType.IsAssignableFrom(viewType))
				{
					var method = viewType.GetMethod(ConfigureMethodName, BindingFlags.Public | BindingFlags.Instance);

					if (method != null)
					{
						method.Invoke(obj, new object[] { args });
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Instantiates a prefab making sure it has a component of type <typeparamref name="TComponent"/> attached.
		/// </summary>
		/// <typeparam name="TComponent">Component type to instantiate.</typeparam>
		/// <param name="prefab">The prefab to instantiate.</param>
		/// <param name="parent">A transform to parent the created game object to (or <see langword="null"/>).</param>
		/// <param name="serviceProvider">A service provider to get component dependencies from (or <see langword="null"/>).</param>
		/// <param name="args">Aguments to get component dependencies from (or <see langword="null"/>).</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="prefab"/> is <see langword="null"/>.</exception>
		/// <returns>Returns the component instance created.</returns>
		/// <seealso cref="InstantiatePrefab(Type, GameObject, Transform, IServiceProvider, object[])"/>
		public static TComponent InstantiatePrefab<TComponent>(GameObject prefab, Transform parent, IServiceProvider serviceProvider = null, object[] args = null) where TComponent : Component
		{
			return (TComponent)InstantiatePrefab(typeof(TComponent), prefab, parent, serviceProvider, args);
		}

		/// <summary>
		/// Instantiates a prefab making sure it has a component of type <paramref name="componentType"/> attached.
		/// </summary>
		/// <param name="componentType">Type of the component to instantiate.</param>
		/// <param name="prefab">The prefab to instantiate.</param>
		/// <param name="parent">A transform to parent the created game object to (or <see langword="null"/>).</param>
		/// <param name="serviceProvider">A service provider to get component dependencies from (or <see langword="null"/>).</param>
		/// <param name="args">Aguments to get component dependencies from (or <see langword="null"/>).</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="componentType"/> or <paramref name="prefab"/> is <see langword="null"/>.</exception>
		/// <returns>Returns the component instance created.</returns>
		/// <seealso cref="InstantiatePrefab{TComponent}(GameObject, Transform, IServiceProvider, object[])"/>
		public static Component InstantiatePrefab(Type componentType, GameObject prefab, Transform parent, IServiceProvider serviceProvider = null, object[] args = null)
		{
			if (componentType is null)
			{
				throw new ArgumentNullException(nameof(componentType));
			}

			if (prefab is null)
			{
				throw new ArgumentNullException(nameof(prefab));
			}

			var go = GameObject.Instantiate(prefab, parent, false);
			var c = go.GetComponent(componentType);

			if (c is null)
			{
				c = go.AddComponent(componentType);
			}

			if (serviceProvider != null)
			{
				var initMethod = componentType.GetMethod(InitializeMethodName, BindingFlags.Instance | BindingFlags.Public);

				if (initMethod != null && TryGetMethodArguments(initMethod, serviceProvider, args, out var argValues))
				{
					initMethod.Invoke(c, argValues);
				}
			}

			return c;
		}

		/// <summary>
		/// Creates an instance of the specified <typeparamref name="TComponent"/>, attaches it to the specified game object, then
		/// initializes it using values from <paramref name="serviceProvider"/> and <paramref name="args"/> as arguments.
		/// Values from <paramref name="args"/> have priority over values retrieved from <paramref name="serviceProvider"/>.
		/// </summary>
		/// <typeparam name="T">The type to instantiate.</typeparam>
		/// <param name="serviceProvider">A service provider to get argument values from.</param>
		/// <param name="args">Agument values to use.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="go"/> is <see langword="null"/>.</exception>
		/// <returns>Returns the component created.</returns>
		/// <seealso cref="CreateComponent(Type, GameObject, IServiceProvider, object[])"/>
		public static TComponent CreateComponent<TComponent>(GameObject go, IServiceProvider serviceProvider = null, object[] args = null) where TComponent : Component
		{
			return (TComponent)CreateComponent(typeof(TComponent), go, serviceProvider, args);
		}

		/// <summary>
		/// Creates an instance of the specified <paramref name="type"/>, attaches it to the specified game object, then
		/// initializes it using values from <paramref name="serviceProvider"/> and <paramref name="args"/> as arguments.
		/// Values from <paramref name="args"/> have priority over values retrieved from <paramref name="serviceProvider"/>.
		/// </summary>
		/// <param name="type">Type to instantiate.</param>
		/// <param name="serviceProvider">A service provider to get argument values from.</param>
		/// <param name="args">Agument values to use.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="type"/> or <paramref name="go"/> is <see langword="null"/>.</exception>
		/// <returns>Returns the component created.</returns>
		/// <seealso cref="CreateComponent{TComponent}(GameObject, IServiceProvider, object[])"/>
		public static Component CreateComponent(Type type, GameObject go, IServiceProvider serviceProvider = null, object[] args = null)
		{
			if (go is null)
			{
				throw new ArgumentNullException(nameof(go));
			}

			var c = go.AddComponent(type);

			if (serviceProvider != null)
			{
				var initMethod = type.GetMethod(InitializeMethodName, BindingFlags.Instance | BindingFlags.Public);

				if (initMethod != null && TryGetMethodArguments(initMethod, serviceProvider, args, out var argValues))
				{
					initMethod.Invoke(c, argValues);
				}
			}

			return c;
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
		public static object CreateInstance(Type type, IServiceProvider serviceProvider, object[] args = null)
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
