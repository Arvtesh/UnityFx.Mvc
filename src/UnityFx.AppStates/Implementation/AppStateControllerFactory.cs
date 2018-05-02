// Copyright (c) Alexander Bogarsukov.
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

		public AppViewController CreateController(Type controllerType, AppState state)
		{
			Debug.Assert(controllerType != null);
			Debug.Assert(controllerType.IsSubclassOf(typeof(AppViewController)));
			Debug.Assert(state != null);

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
						args[i] = GetServiceInstance(parameters[i].ParameterType, state);
					}

					return c.Invoke(args) as AppViewController;
				}
				else
				{
					return Activator.CreateInstance(controllerType) as AppViewController;
				}
			}
			catch (TargetInvocationException e)
			{
				throw e.InnerException;
			}
		}

		#endregion

		#region implementation

		private object GetServiceInstance(Type serviceType, AppState state)
		{
			if (serviceType == typeof(AppState))
			{
				return state;
			}
			else if (serviceType == typeof(IServiceProvider))
			{
				return _serviceProvider;
			}
			else if (serviceType == typeof(AppView))
			{
				return state.View;
			}
			else if (serviceType == typeof(IAppStateService))
			{
				return state.StateManager;
			}

			return _serviceProvider.GetService(serviceType);
		}

		#endregion
	}
}
