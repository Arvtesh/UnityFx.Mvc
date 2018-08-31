// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using System.Reflection;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Default implementation of <see cref="IPresentableFactory"/>.
	/// </summary>
	internal sealed class DefaultPresentableFactory : IPresentableFactory
	{
		#region data

		private readonly IServiceProvider _serviceProvider;

		#endregion

		#region interface

		public DefaultPresentableFactory(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		#endregion

		#region IAppStateControllerFactory

		public IPresentable CreateController(Type controllerType, IPresentableContext context)
		{
			Debug.Assert(controllerType != null);
			Debug.Assert(controllerType.IsSubclassOf(typeof(IPresentable)));
			Debug.Assert(context != null);

			try
			{
				var constructors = controllerType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

				if (constructors.Length > 0)
				{
					var c = constructors[0];
					var parameters = c.GetParameters();
					var args = new object[parameters.Length];

					for (var i = 0; i < args.Length; i++)
					{
						args[i] = GetServiceInstance(parameters[i].ParameterType, context);
					}

					return c.Invoke(args) as IPresentable;
				}
				else
				{
					return Activator.CreateInstance(controllerType) as IPresentable;
				}
			}
			catch (TargetInvocationException e)
			{
				throw e.InnerException;
			}
		}

		#endregion

		#region implementation

		private object GetServiceInstance(Type serviceType, IPresentableContext context)
		{
			if (serviceType == typeof(IPresentableContext))
			{
				return context;
			}
			else if (serviceType == typeof(IServiceProvider))
			{
				return _serviceProvider;
			}

			return _serviceProvider.GetService(serviceType);
		}

		#endregion
	}
}
