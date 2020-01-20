// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A builder of a <see cref="IPresenter"/> instance.
	/// </summary>
	/// <seealso cref="IPresenter"/>
	public interface IPresenterBuilder
	{
		/// <summary>
		/// Gets the <see cref="IServiceProvider"/> that provides access to the application's service container.
		/// </summary>
		IServiceProvider ServiceProvider { get; }

		/// <summary>
		/// Gets a key/value collection that can be used to share data between middleware.
		/// </summary>
		IDictionary<string, object> Properties { get; }

		/// <summary>
		/// Sets a <see cref="IViewFactory"/> instace to use.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="viewFactory"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the view factory is already set.</exception>
		/// <seealso cref="UseViewControllerFactory(IViewControllerFactory)"/>
		/// <seealso cref="Build"/>
		IPresenterBuilder UseViewFactory(IViewFactory viewFactory);

		/// <summary>
		/// Sets a <see cref="IViewControllerFactory"/> instace to use.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="viewControllerFactory"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the view controller factory is already set.</exception>
		/// <seealso cref="UseViewFactory(IViewFactory)"/>
		/// <seealso cref="Build"/>
		IPresenterBuilder UseViewControllerFactory(IViewControllerFactory viewControllerFactory);

		/// <summary>
		/// Adds a <see cref="PresentDelegate"/> to presenter middleware chain.
		/// </summary>
		/// <param name="presentDelegate">The middleware to add.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="presentDelegate"/> is <see langword="null"/>.</exception>
		/// <seealso cref="Build"/>
		IPresenterBuilder UsePresentDelegate(PresentDelegate presentDelegate);

		/// <summary>
		/// Creates a <see cref="MonoBehaviour"/>-based implementation of <see cref="IPresenter"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if presenter cannot be constructed (for instance, <see cref="IViewFactory"/> is not set and cannot be located).</exception>
		IPresentService Build();
	}
}
