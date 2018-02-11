// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A factory for state controllers instances.
	/// </summary>
	/// <seealso cref="IAppState"/>
	public interface IAppStateControllerFactory
	{
		/// <summary>
		/// Creates a new instance of state controller and injects its dependencies (if needed).
		/// </summary>
		/// <param name="controllerType">Type of the controller to be created.</param>
		/// <param name="stateContext">State context.</param>
		/// <param name="serviceProvider">A service provider instance. Should be used as a source for the controller dependencies.</param>
		/// <returns></returns>
		IAppStateController CreateController(Type controllerType, IAppStateContext stateContext, IServiceProvider serviceProvider);
	}
}