// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Provides methods for creation and disposal of view controllers.
	/// </summary>
	/// <seealso cref="IViewController"/>
	public interface IViewControllerFactory
	{
		/// <summary>
		/// Creates a scoped <see cref="IServiceProvider"/> instance.
		/// </summary>
		/// <param name="serviceProvider">A <see cref="IServiceProvider"/> used to resolve controller dependencies.</param>
		/// <returns>A disposable scope created or <see langword="null"/>.</returns>
		IDisposable CreateScope(ref IServiceProvider serviceProvider);

		/// <summary>
		/// Creates a new instance of <see cref="IViewController"/> and injects its dependencies.
		/// </summary>
		/// <param name="controllerType">Type of the controller to be created.</param>
		/// <param name="args">Additional arguments to use when resolving controller dependencies.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controllerType"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if <paramref name="controllerType"/> is not a valid controller type (for instance, <paramref name="controllerType"/> is abstract).</exception>
		/// <returns>The created controller instance.</returns>
		/// <seealso cref="Release(IViewController)"/>
		IViewController Create(Type controllerType, params object[] args);

		/// <summary>
		/// Releases a controller after it has been dismissed.
		/// </summary>
		/// <param name="controller">The controller to be disposed.</param>
		/// <seealso cref="Create(Type, object[])"/>
		void Release(IViewController controller);
	}
}
