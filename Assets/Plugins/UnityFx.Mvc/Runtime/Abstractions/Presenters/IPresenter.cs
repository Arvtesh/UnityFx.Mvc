// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// An object capable of presenting view controllers.
	/// </summary>
	/// <remarks>
	/// Please note that users of this type should not access <see cref="IViewController"/> or its <see cref="IView"/> directly.
	/// Instead, <see cref="IPresentResult"/> can be used to track the controller lifetime events or to dismiss it.
	/// </remarks>
	/// <seealso cref="IViewController"/>
	/// <seealso cref="IViewControllerEvents"/>
	/// <seealso cref="IPresentContext"/>
	public interface IPresenter
	{
		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		/// <param name="controllerType">Type of the view controller to present.</param>
		/// <param name="args">Controller arguments.</param>
		/// <param name="presentOptions">Present flags.</param>
		/// <param name="parent">Parent transform for the view (or <see langword="null"/>).</param>
		/// <returns>An object that can be used to track the operation propress.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controllerType"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="controllerType"/> cannot be used to instantiate the controller (for instance, it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the presenter is disposed.</exception>
		IPresentResult Present(Type controllerType, PresentArgs args, PresentOptions presentOptions, Transform parent);
	}
}
