// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A generic view controller.
	/// </summary>
	/// <remarks>
	/// As the name states, main responsibility of a view controller is managing its view.
	/// Controllers are created via a <see cref="IViewControllerFactory"/> instance.
	/// </remarks>
	/// <seealso cref="IView"/>
	/// <seealso cref="IPresenter"/>
	/// <seealso cref="IPresentContext"/>
	/// <seealso cref="IViewControllerFactory"/>
	/// <seealso cref="IViewControllerResult{TResult}"/>
	/// <seealso href="https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93controller"/>
	public interface IViewController
	{
		/// <summary>
		/// Gets a view managed by the controller. Should never returns <see langword="null"/>.
		/// </summary>
		IView View { get; }
	}
}
