﻿// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A factory for view controllers instances.
	/// </summary>
	/// <seealso cref="IViewController"/>
	public interface IViewControllerFactory
	{
		/// <summary>
		/// Creates a new instance of state controller and injects its dependencies.
		/// </summary>
		/// <param name="controllerType">Type of the controller to be created.</param>
		/// <param name="context">The controller context.</param>
		/// <returns>The created controller.</returns>
		IViewController CreateController(Type controllerType, IViewControllerContext context);
	}
}
