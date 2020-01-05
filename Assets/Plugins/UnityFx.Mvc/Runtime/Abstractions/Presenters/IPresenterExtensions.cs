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
		public static IPresentResult Present(this IPresenter presenter, Type controllerType)
		{
			return presenter.Present(controllerType, null, PresentOptions.None, null);
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
			return presenter.Present(controllerType, null, PresentOptions.None, null).Task;
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
		public static IPresentResult Present(this IPresenter presenter, Type controllerType, PresentArgs args)
		{
			return presenter.Present(controllerType, args, PresentOptions.None, null);
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
			return presenter.Present(controllerType, args, PresentOptions.None, null).Task;
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <param name="presenter">The presenter.</param>
		/// <param name="controllerType">Type of the view controller to present.</param>
		/// <param name="options">Present options.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controllerType"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="controllerType"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		/// <seealso cref="Present(Type)"/>
		public static IPresentResult Present(this IPresenter presenter, Type controllerType, PresentArgs args, PresentOptions options)
		{
			return presenter.Present(controllerType, args, options, null);
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <param name="presenter">The presenter.</param>
		/// <param name="controllerType">Type of the view controller to present.</param>
		/// <param name="options">Present options.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controllerType"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="controllerType"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		/// <seealso cref="Present(Type)"/>
		public static Task PresentAsync(this IPresenter presenter, Type controllerType, PresentArgs args, PresentOptions options)
		{
			return presenter.Present(controllerType, args, options, null).Task;
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <param name="presenter">The presenter.</param>
		/// <param name="controllerType">Type of the view controller to present.</param>
		/// <param name="args">Controller arguments.</param>
		/// <param name="options">Present options.</param>
		/// <param name="transform">Parent transform of the controller view.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controllerType"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="controllerType"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		/// <seealso cref="Present(Type)"/>
		public static Task PresentAsync(this IPresenter presenter, Type controllerType, PresentArgs args, PresentOptions options, Transform transform)
		{
			return presenter.Present(controllerType, args, options, transform).Task;
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
			return presenter.Present(controllerType, null, options, null);
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
			return presenter.Present(controllerType, null, options, null).Task;
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
		/// <seealso cref="Present(Type)"/>
		public static IPresentResult Present(this IPresenter presenter, Type controllerType, PresentOptions options, Transform transform)
		{
			return presenter.Present(controllerType, null, options, transform);
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
		/// <seealso cref="Present(Type)"/>
		public static Task PresentAsync(this IPresenter presenter, Type controllerType, PresentOptions options, Transform transform)
		{
			return presenter.Present(controllerType, null, options, transform).Task;
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <typeparam name="TController">Type of the controller to instantiate.</typeparam>
		/// <param name="presenter">The presenter.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static IPresentResultOf<TController> Present<TController>(this IPresenter presenter) where TController : IViewController
		{
			return (IPresentResultOf<TController>)presenter.Present(typeof(TController), null, PresentOptions.None, null);
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
			return presenter.Present(typeof(TController), null, PresentOptions.None, null).Task;
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
		public static IPresentResultOf<TController> Present<TController>(this IPresenter presenter, PresentArgs args) where TController : IViewController
		{
			return (IPresentResultOf<TController>)presenter.Present(typeof(TController), args, PresentOptions.None, null);
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
			return presenter.Present(typeof(TController), args, PresentOptions.None, null).Task;
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <typeparam name="TController">Type of the controller to instantiate.</typeparam>
		/// <param name="presenter">The presenter.</param>
		/// <param name="options">Present options.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static IPresentResultOf<TController> Present<TController>(this IPresenter presenter, PresentArgs args, PresentOptions options) where TController : IViewController
		{
			return (IPresentResultOf<TController>)presenter.Present(typeof(TController), args, options, null);
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <typeparam name="TController">Type of the controller to instantiate.</typeparam>
		/// <param name="presenter">The presenter.</param>
		/// <param name="options">Present options.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static Task PresentAsync<TController>(this IPresenter presenter, PresentArgs args, PresentOptions options) where TController : IViewController
		{
			return presenter.Present(typeof(TController), args, options, null).Task;
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <typeparam name="TController">Type of the controller to instantiate.</typeparam>
		/// <param name="presenter">The presenter.</param>
		/// <param name="options">Present options.</param>
		/// <param name="transform">Parent transform of the controller view.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static IPresentResultOf<TController> Present<TController>(this IPresenter presenter, PresentArgs args, PresentOptions options, Transform transform) where TController : IViewController
		{
			return (IPresentResultOf<TController>)presenter.Present(typeof(TController), args, options, transform);
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <typeparam name="TController">Type of the controller to instantiate.</typeparam>
		/// <param name="presenter">The presenter.</param>
		/// <param name="options">Present options.</param>
		/// <param name="transform">Parent transform of the controller view.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static Task PresentAsync<TController>(this IPresenter presenter, PresentArgs args, PresentOptions options, Transform transform) where TController : IViewController
		{
			return presenter.Present(typeof(TController), args, options, transform).Task;
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
		public static IPresentResultOf<TController> Present<TController>(this IPresenter presenter, PresentOptions options, Transform transform) where TController : IViewController
		{
			return (IPresentResultOf<TController>)presenter.Present(typeof(TController), null, options, transform);
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
			return presenter.Present(typeof(TController), null, options, transform).Task;
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <typeparam name="TController">Type of the controller to instantiate.</typeparam>
		/// <typeparam name="TResult">Type of the controller result value.</typeparam>
		/// <param name="presenter">The presenter.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="InvalidCastException">Thrown if the <typeparamref name="TResult"/> does not match result type of the <typeparamref name="TController"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static IPresentResultOf<TController, TResult> Present<TController, TResult>(this IPresenter presenter) where TController : IViewController, IViewControllerResult<TResult>
		{
			return (IPresentResultOf<TController, TResult>)presenter.Present(typeof(TController), null, PresentOptions.None, null);
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <typeparam name="TController">Type of the controller to instantiate.</typeparam>
		/// <typeparam name="TResult">Type of the controller result value.</typeparam>
		/// <param name="presenter">The presenter.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="InvalidCastException">Thrown if the <typeparamref name="TResult"/> does not match result type of the <typeparamref name="TController"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static Task<TResult> PresentAsync<TController, TResult>(this IPresenter presenter) where TController : IViewController, IViewControllerResult<TResult>
		{
			return ((IPresentResultOf<TController, TResult>)presenter.Present(typeof(TController), null, PresentOptions.None, null)).Task;
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <typeparam name="TController">Type of the controller to instantiate.</typeparam>
		/// <typeparam name="TResult">Type of the controller result value.</typeparam>
		/// <param name="presenter">The presenter.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="InvalidCastException">Thrown if the <typeparamref name="TResult"/> does not match result type of the <typeparamref name="TController"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static IPresentResultOf<TController, TResult> Present<TController, TResult>(this IPresenter presenter, PresentArgs args) where TController : IViewController, IViewControllerResult<TResult>
		{
			return (IPresentResultOf<TController, TResult>)presenter.Present(typeof(TController), args, PresentOptions.None, null);
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <typeparam name="TController">Type of the controller to instantiate.</typeparam>
		/// <typeparam name="TResult">Type of the controller result value.</typeparam>
		/// <param name="presenter">The presenter.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="InvalidCastException">Thrown if the <typeparamref name="TResult"/> does not match result type of the <typeparamref name="TController"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static Task<TResult> PresentAsync<TController, TResult>(this IPresenter presenter, PresentArgs args) where TController : IViewController, IViewControllerResult<TResult>
		{
			return ((IPresentResultOf<TController, TResult>)presenter.Present(typeof(TController), args, PresentOptions.None, null)).Task;
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <typeparam name="TController">Type of the controller to instantiate.</typeparam>
		/// <typeparam name="TResult">Type of the controller result value.</typeparam>
		/// <param name="presenter">The presenter.</param>
		/// <param name="args">Controller arguments.</param>
		/// <param name="options">Present options.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="InvalidCastException">Thrown if the <typeparamref name="TResult"/> does not match result type of the <typeparamref name="TController"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static IPresentResultOf<TController, TResult> Present<TController, TResult>(this IPresenter presenter, PresentArgs args, PresentOptions options) where TController : IViewController, IViewControllerResult<TResult>
		{
			return (IPresentResultOf<TController, TResult>)presenter.Present(typeof(TController), args, options, null);
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <typeparam name="TController">Type of the controller to instantiate.</typeparam>
		/// <typeparam name="TResult">Type of the controller result value.</typeparam>
		/// <param name="presenter">The presenter.</param>
		/// <param name="args">Controller arguments.</param>
		/// <param name="options">Present options.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="InvalidCastException">Thrown if the <typeparamref name="TResult"/> does not match result type of the <typeparamref name="TController"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static Task<TResult> PresentAsync<TController, TResult>(this IPresenter presenter, PresentArgs args, PresentOptions options) where TController : IViewController, IViewControllerResult<TResult>
		{
			return ((IPresentResultOf<TController, TResult>)presenter.Present(typeof(TController), args, options, null)).Task;
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <typeparam name="TController">Type of the controller to instantiate.</typeparam>
		/// <typeparam name="TResult">Type of the controller result value.</typeparam>
		/// <param name="presenter">The presenter.</param>
		/// <param name="options">Present options.</param>
		/// <param name="transform">Parent transform of the controller view.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="InvalidCastException">Thrown if the <typeparamref name="TResult"/> does not match result type of the <typeparamref name="TController"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static IPresentResultOf<TController, TResult> Present<TController, TResult>(this IPresenter presenter, PresentArgs args, PresentOptions options, Transform transform) where TController : IViewController, IViewControllerResult<TResult>
		{
			return (IPresentResultOf<TController, TResult>)presenter.Present(typeof(TController), args, options, transform);
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <typeparam name="TController">Type of the controller to instantiate.</typeparam>
		/// <typeparam name="TResult">Type of the controller result value.</typeparam>
		/// <param name="presenter">The presenter.</param>
		/// <param name="options">Present options.</param>
		/// <param name="transform">Parent transform of the controller view.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="InvalidCastException">Thrown if the <typeparamref name="TResult"/> does not match result type of the <typeparamref name="TController"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static Task<TResult> PresentAsync<TController, TResult>(this IPresenter presenter, PresentArgs args, PresentOptions options, Transform transform) where TController : IViewController, IViewControllerResult<TResult>
		{
			return ((IPresentResultOf<TController, TResult>)presenter.Present(typeof(TController), args, options, transform)).Task;
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <typeparam name="TController">Type of the controller to instantiate.</typeparam>
		/// <typeparam name="TResult">Type of the controller result value.</typeparam>
		/// <param name="presenter">The presenter.</param>
		/// <param name="options">Present options.</param>
		/// <param name="transform">Parent transform of the controller view.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="InvalidCastException">Thrown if the <typeparamref name="TResult"/> does not match result type of the <typeparamref name="TController"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static IPresentResultOf<TController, TResult> Present<TController, TResult>(this IPresenter presenter, PresentOptions options, Transform transform) where TController : IViewController, IViewControllerResult<TResult>
		{
			return (IPresentResultOf<TController, TResult>)presenter.Present(typeof(TController), null, options, transform);
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <typeparam name="TController">Type of the controller to instantiate.</typeparam>
		/// <typeparam name="TResult">Type of the controller result value.</typeparam>
		/// <param name="presenter">The presenter.</param>
		/// <param name="options">Present options.</param>
		/// <param name="transform">Parent transform of the controller view.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate the controller (for instance it is abstract type).</exception>
		/// <exception cref="InvalidCastException">Thrown if the <typeparamref name="TResult"/> does not match result type of the <typeparamref name="TController"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		public static Task<TResult> PresentAsync<TController, TResult>(this IPresenter presenter, PresentOptions options, Transform transform) where TController : IViewController, IViewControllerResult<TResult>
		{
			return ((IPresentResultOf<TController, TResult>)presenter.Present(typeof(TController), null, options, transform)).Task;
		}
	}
}
