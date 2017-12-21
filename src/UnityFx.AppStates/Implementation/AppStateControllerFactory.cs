// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using System.Reflection;

namespace UnityFx.App
{
	/// <summary>
	/// Default implementation of <see cref="IAppStateControllerFactory"/>.
	/// </summary>
	internal sealed class AppStateControllerFactory : IAppStateControllerFactory
	{
		#region interface
		#endregion

		#region IAppStateControllerFactory

		public IAppStateController CreateController(Type controllerType, IAppStateContext stateContext, IServiceProvider serviceProvider)
		{
			try
			{
				// TODO: use IServiceProvider to resolve controller constructor parameters
				return Activator.CreateInstance(controllerType) as IAppStateController;
			}
			catch (TargetInvocationException e)
			{
				throw e.InnerException;
			}
		}

		#endregion

		#region implementation
		#endregion
	}
}
