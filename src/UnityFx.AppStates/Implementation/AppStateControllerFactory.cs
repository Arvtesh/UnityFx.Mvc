﻿// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using System.Reflection;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Default implementation of <see cref="IAppStateControllerFactory"/>.
	/// </summary>
	internal sealed class AppStateControllerFactory : IAppStateControllerFactory
	{
		#region data

		private readonly IServiceProvider _serviceProvider;

		#endregion

		#region interface

		public AppStateControllerFactory(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		#endregion

		#region IAppStateControllerFactory

		public IAppStateController CreateController(Type controllerType, IAppStateContext stateContext)
		{
			Debug.Assert(controllerType != null);
			Debug.Assert(stateContext != null);

			try
			{
				var constructors = controllerType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

				if (constructors.Length > 0)
				{
					var c = constructors[0];
					var parameters = c.GetParameters();
					var args = new object[parameters.Length];

					for (int i = 0; i < args.Length; i++)
					{
						args[i] = GetServiceInstance(parameters[i].ParameterType, stateContext);
					}

					return c.Invoke(args) as IAppStateController;
				}
				else
				{
					return Activator.CreateInstance(controllerType) as IAppStateController;
				}
			}
			catch (TargetInvocationException e)
			{
				throw e.InnerException;
			}
		}

		#endregion

		#region implementation

		private object GetServiceInstance(Type serviceType, IAppStateContext stateContext)
		{
			if (serviceType == typeof(IAppStateContext))
			{
				return stateContext;
			}
			else if (serviceType == typeof(IServiceProvider))
			{
				return _serviceProvider;
			}
			else if (serviceType == typeof(IAppState))
			{
				return stateContext.State;
			}
			else if (serviceType == typeof(IAppView))
			{
				return stateContext.View;
			}
			else if (serviceType == typeof(IAppStateManager))
			{
				return stateContext.StateManager;
			}

			return _serviceProvider.GetService(serviceType);
		}

		#endregion
	}
}
