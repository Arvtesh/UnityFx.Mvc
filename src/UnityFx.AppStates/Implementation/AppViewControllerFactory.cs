// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using System.Reflection;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Default implementation of <see cref="IAppViewControllerFactory"/>.
	/// </summary>
	internal sealed class AppViewControllerFactory : IAppViewControllerFactory
	{
		#region data

		private readonly IAppStateService _stateManager;
		private readonly IAppViewService _viewManager;
		private readonly IServiceProvider _serviceProvider;

		#endregion

		#region interface

		public AppViewControllerFactory(IAppStateService stateManager, IAppViewService viewManager, IServiceProvider serviceProvider)
		{
			_stateManager = stateManager;
			_viewManager = viewManager;
			_serviceProvider = serviceProvider;
		}

		#endregion

		#region IAppStateControllerFactory

		public AppViewController CreateController(Type controllerType, IAppViewControllerContext context)
		{
			Debug.Assert(controllerType != null);
			Debug.Assert(controllerType.IsSubclassOf(typeof(AppViewController)));
			Debug.Assert(context != null);

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
						args[i] = GetServiceInstance(parameters[i].ParameterType, context);
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

		private object GetServiceInstance(Type serviceType, IAppViewControllerContext context)
		{
			if (serviceType == typeof(IAppViewControllerContext))
			{
				return context;
			}
			else if (serviceType == typeof(IServiceProvider))
			{
				return _serviceProvider;
			}
			else if (serviceType == typeof(IAppStateService))
			{
				return _stateManager;
			}
			else if (serviceType == typeof(IAppViewService))
			{
				return _viewManager;
			}

			return _serviceProvider.GetService(serviceType);
		}

		#endregion
	}
}
