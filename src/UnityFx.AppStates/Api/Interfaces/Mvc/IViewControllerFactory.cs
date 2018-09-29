// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A factory of <see cref="IViewController"/> instances.
	/// </summary>
	/// <seealso cref="IViewController"/>
	public interface IViewControllerFactory
	{
		/// <summary>
		/// Creates a scoped <see cref="IServiceProvider"/>.
		/// </summary>
		/// <param name="serviceProvider">A service provider passed to the controller created by the factory.</param>
		/// <returns>A disposable scope created or <see langword="null"/>.</returns>
		IDisposable CreateControllerScope(ref IServiceProvider serviceProvider);

		/// <summary>
		/// Creates a new instance of <see cref="IViewController"/> and injects its dependencies.
		/// </summary>
		/// <param name="controllerType">Type of the controller to be created.</param>
		/// <param name="context">The controller context.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="controllerType"/> or <paramref name="context"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if <paramref name="controllerType"/> is not a valid controller type.</exception>
		/// <returns>The created controller instance.</returns>
		IViewController CreateController(Type controllerType, IViewControllerContext context);
	}
}
