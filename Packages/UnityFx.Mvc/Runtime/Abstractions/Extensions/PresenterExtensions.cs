// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Extensions of <see cref="IPresenter"/> interface.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class PresenterExtensions
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
		public static IPresentResult Present(this IPresenter presenter, Type controllerType)
		{
			return presenter.Present(controllerType, null);
		}

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
		public static Task PresentAsync(this IPresenter presenter, Type controllerType)
		{
			return presenter.Present(controllerType, null).Task;
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <param name="presenter">The presenter.</param>
		/// <param name="controllerType">Type of the view controller to present.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controllerType"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="controllerType"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		/// <seealso cref="Present(Type)"/>
		public static Task PresentAsync(this IPresenter presenter, Type controllerType, PresentArgs args)
		{
			return presenter.Present(controllerType, args).Task;
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <param name="presenter">The presenter.</param>
		/// <param name="controllerType">Type of the view controller to present.</param>
		/// <param name="options">Present options.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controllerType"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="controllerType"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		/// <seealso cref="Present(Type)"/>
		public static IPresentResult Present(this IPresenter presenter, Type controllerType, PresentOptions options)
		{
			return presenter.Present(controllerType, new PresentArgs() { PresentOptions = options });
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <param name="presenter">The presenter.</param>
		/// <param name="controllerType">Type of the view controller to present.</param>
		/// <param name="options">Present options.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controllerType"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="controllerType"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		/// <seealso cref="Present(Type)"/>
		public static Task PresentAsync(this IPresenter presenter, Type controllerType, PresentOptions options)
		{
			return presenter.Present(controllerType, new PresentArgs() { PresentOptions = options }).Task;
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <param name="presenter">The presenter.</param>
		/// <param name="controllerType">Type of the view controller to present.</param>
		/// <param name="options">Present options.</param>
		/// <param name="transform">Parent transform of the controller view.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controllerType"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="controllerType"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		/// <seealso cref="PresentAsync(IPresenter, Type, PresentOptions, Transform)"/>
		public static IPresentResult Present(this IPresenter presenter, Type controllerType, PresentOptions options, Transform transform)
		{
			return presenter.Present(controllerType, new PresentArgs() { PresentOptions = options, Transform = transform });
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <param name="presenter">The presenter.</param>
		/// <param name="controllerType">Type of the view controller to present.</param>
		/// <param name="options">Present options.</param>
		/// <param name="transform">Parent transform of the controller view.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controllerType"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="controllerType"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		/// <seealso cref="Present(IPresenter, Type, PresentOptions, Transform)"/>
		public static Task PresentAsync(this IPresenter presenter, Type controllerType, PresentOptions options, Transform transform)
		{
			return presenter.Present(controllerType, new PresentArgs() { PresentOptions = options, Transform = transform }).Task;
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <typeparam name="TController">Type of the controller to instantiate.</typeparam>
		/// <param name="presenter">The presenter.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static IPresentResult Present<TController>(this IPresenter presenter) where TController : IViewController
		{
			return presenter.Present(typeof(TController), null);
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <typeparam name="TController">Type of the controller to instantiate.</typeparam>
		/// <param name="presenter">The presenter.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static Task PresentAsync<TController>(this IPresenter presenter) where TController : IViewController
		{
			return presenter.Present(typeof(TController), null).Task;
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <typeparam name="TController">Type of the controller to instantiate.</typeparam>
		/// <param name="presenter">The presenter.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static IPresentResult Present<TController>(this IPresenter presenter, PresentArgs args) where TController : IViewController
		{
			return presenter.Present(typeof(TController), args);
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <typeparam name="TController">Type of the controller to instantiate.</typeparam>
		/// <param name="presenter">The presenter.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static Task PresentAsync<TController>(this IPresenter presenter, PresentArgs args) where TController : IViewController
		{
			return presenter.Present(typeof(TController), args).Task;
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <typeparam name="TController">Type of the controller to instantiate.</typeparam>
		/// <param name="presenter">The presenter.</param>
		/// <param name="options">Present options.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static IPresentResult Present<TController>(this IPresenter presenter, PresentOptions options) where TController : IViewController
		{
			return presenter.Present(typeof(TController), new PresentArgs() { PresentOptions = options });
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <typeparam name="TController">Type of the controller to instantiate.</typeparam>
		/// <param name="presenter">The presenter.</param>
		/// <param name="options">Present options.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static Task PresentAsync<TController>(this IPresenter presenter, PresentOptions options) where TController : IViewController
		{
			return presenter.Present(typeof(TController), new PresentArgs() { PresentOptions = options }).Task;
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <typeparam name="TController">Type of the controller to instantiate.</typeparam>
		/// <param name="presenter">The presenter.</param>
		/// <param name="options">Present options.</param>
		/// <param name="transform">Parent transform of the controller view.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static IPresentResult Present<TController>(this IPresenter presenter, PresentOptions options, Transform transform) where TController : IViewController
		{
			return presenter.Present(typeof(TController), new PresentArgs() { PresentOptions = options, Transform = transform });
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <typeparam name="TController">Type of the controller to instantiate.</typeparam>
		/// <param name="presenter">The presenter.</param>
		/// <param name="options">Present options.</param>
		/// <param name="transform">Parent transform of the controller view.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static Task PresentAsync<TController>(this IPresenter presenter, PresentOptions options, Transform transform) where TController : IViewController
		{
			return presenter.Present(typeof(TController), new PresentArgs() { PresentOptions = options, Transform = transform }).Task;
		}
	}
}
