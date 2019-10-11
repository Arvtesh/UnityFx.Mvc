// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Extensions of <see cref="IPresenter"/> interface.
	/// </summary>
	public static class IPresenterExtensions
	{
		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <param name="presenter">The presenter.</param>
		/// <param name="controllerType">Type of the view controller to present.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controllerType"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="controllerType"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		/// <seealso cref="Present(Type)"/>
		public static IPresentResult PresentAsync(this IPresenter presenter, Type controllerType)
		{
			return presenter.PresentAsync(controllerType, null);
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <param name="presenter">The presenter.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static IPresentResult<TController> PresentAsync<TController>(this IPresenter presenter) where TController : IViewController
		{
			return (IPresentResult<TController>)presenter.PresentAsync(typeof(TController), null);
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <param name="presenter">The presenter.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static IPresentResult<TController> PresentAsync<TController>(this IPresenter presenter, PresentArgs args) where TController : IViewController
		{
			return (IPresentResult<TController>)presenter.PresentAsync(typeof(TController), args);
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <param name="presenter">The presenter.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static IPresentResult<TController, TResult> PresentAsync<TController, TResult>(this IPresenter presenter, PresentArgs args) where TController : IViewController
		{
			return (IPresentResult<TController, TResult>)presenter.PresentAsync(typeof(TController), args);
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <param name="presenter">The presenter.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static IPresentResult<TController, TResult> PresentAsync<TController, TResult>(this IPresenter presenter) where TController : IViewController
		{
			return (IPresentResult<TController, TResult>)presenter.PresentAsync(typeof(TController), null);
		}
	}
}
