// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// An object capable of presenting view controllers.
	/// </summary>
	/// <seealso cref="AppViewController"/>
	public interface IPresenter
	{
		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <param name="controllerType">Type of the view controller to present.</param>
		/// <param name="options">Presentation options.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controllerType"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="controllerType"/> cannot be used to instantiate state controller (for instance it is abstract type).</exception>
		/// <exception cref="InvalidOperationException">Too many operations are scheduled already.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if either the controller or its parent state is disposed.</exception>
		IAsyncOperation<IPresentable> PresentAsync(Type controllerType, PresentOptions options, PresentArgs args);

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <param name="controllerType">Type of the state controller.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controllerType"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="controllerType"/> cannot be used to instantiate state controller (for instance it is abstract type).</exception>
		/// <exception cref="InvalidOperationException">Too many operations are scheduled already.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if either the controller or its parent state is disposed.</exception>
		IAsyncOperation<IPresentable> PresentAsync(Type controllerType, PresentArgs args);

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <param name="options">Push options.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="InvalidOperationException">Too many operations are scheduled already.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if either the controller or its parent state is disposed.</exception>
		IAsyncOperation<TController> PresentAsync<TController>(PresentOptions options, PresentArgs args) where TController : IPresentable;

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="InvalidOperationException">Too many operations are scheduled already.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if either the controller or its parent state is disposed.</exception>
		IAsyncOperation<TController> PresentAsync<TController>(PresentArgs args) where TController : IPresentable;
	}
}
