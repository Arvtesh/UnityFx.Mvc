// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// An object capable of presenting view controllers.
	/// </summary>
	/// <seealso cref="IViewController"/>
	/// <seealso cref="IViewControllerEvents"/>
	/// <seealso cref="IPresentContext"/>
	/// <seealso cref="IPresentService"/>
	public interface IPresenter
	{
		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <param name="controllerType">Type of the view controller to present.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="controllerType"/> or <paramref name="args"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="controllerType"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		/// <seealso cref="Present(Type)"/>
		IPresentResult Present(Type controllerType, PresentArgs args);

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="args"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		/// <seealso cref="Present{TController}()"/>
		IPresentResult<TController> Present<TController>(PresentArgs args) where TController : IViewController;

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="args"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		/// <seealso cref="Present{TController}(PresentArgs)"/>
		IPresentResult<TController, TResult> Present<TController, TResult>(PresentArgs args) where TController : IViewController<TResult>;
	}
}
