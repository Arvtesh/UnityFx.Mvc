// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Manager of the application flow.
	/// </summary>
	/// <seealso cref="IViewController"/>
	/// <seealso cref="IPresenter"/>
	public interface IPresentService : IPresenter, ICommandTarget, IDisposable
	{
		/// <summary>
		/// Gets the <see cref="IServiceProvider"/> that is used to resolve controller dependencies.
		/// </summary>
		IServiceProvider ServiceProvider { get; }

		/// <summary>
		/// Gets the <see cref="IViewFactory"/> attached.
		/// </summary>
		IViewFactory ViewFactory { get; }

		/// <summary>
		/// Gets a read-only stack of view controllers currently presented.
		/// </summary>
		/// <seealso cref="ActiveController"/>
		IViewControllerCollection Controllers { get; }

		/// <summary>
		/// Gets active controller (if any).
		/// </summary>
		/// <seealso cref="Controllers"/>
		IViewController ActiveController { get; }

		/// <summary>
		/// Gets an information associated with the specified <paramref name="controller"/>.
		/// </summary>
		/// <param name="controller">The controller to query information for</param>
		/// <param name="info">When this method returns, contains the value associated with the specified <paramref name="controller"/>, if the controller is found; otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controller"/> is <see langword="null"/>.</exception>
		/// <returns>Returns <see langword="true"/> if the <paramref name="controller"/> is presented and the info is available; <see langword="false"/> otherwise.</returns>
		/// <seealso cref="IViewControllerInfo"/>
		bool TryGetInfo(IViewController controller, out IViewControllerInfo info);
	}
}
