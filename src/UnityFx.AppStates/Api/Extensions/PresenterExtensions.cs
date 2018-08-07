// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Extensions for <see cref="IPresenter"/> interface.
	/// </summary>
	public static class PresenterExtensions
	{
		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <param name="presenter">The target presenter.</param>
		/// <param name="controllerType">Type of the state controller.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controllerType"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="controllerType"/> cannot be used to instantiate state controller (for instance it is abstract type).</exception>
		/// <exception cref="InvalidOperationException">Too many operations are scheduled already.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if either the controller or its parent state is disposed.</exception>
		public static IAsyncOperation<IPresentable> PresentAsync(this IPresenter presenter, Type controllerType)
		{
			return presenter.PresentAsync(controllerType, PresentArgs.Default);
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <param name="presenter">The target presenter.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="InvalidOperationException">Too many operations are scheduled already.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if either the controller or its parent state is disposed.</exception>
		public static IAsyncOperation<TController> PresentAsync<TController>(this IPresenter presenter) where TController : IPresentable
		{
			return presenter.PresentAsync(typeof(TController), PresentArgs.Default) as IAsyncOperation<TController>;
		}
	}
}
