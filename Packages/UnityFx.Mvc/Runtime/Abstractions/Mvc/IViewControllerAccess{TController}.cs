// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Manages access to a controller of a specific type.
	/// </summary>
	/// <typeparam name="TController">Type of the controller.</typeparam>
	/// <seealso cref="IViewController"/>
	public interface IViewControllerAccess<out TController> where TController : IViewController
	{
		/// <summary>
		/// Gets the view controller.
		/// </summary>
		/// <seealso cref="View"/>
		TController Controller { get; }

		/// <summary>
		/// Gets the view.
		/// </summary>
		/// <seealso cref="Controller"/>
		IView View { get; }
	}
}
