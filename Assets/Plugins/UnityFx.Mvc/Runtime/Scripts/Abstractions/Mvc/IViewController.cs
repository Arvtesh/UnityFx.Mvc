// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A generic view controller.
	/// </summary>
	/// <remarks>
	/// As the name states, main responsibility of a view controller is managing its view.
	/// </remarks>
	/// <seealso cref="IView"/>
	/// <seealso cref="IPresenter"/>
	/// <seealso cref="IPresentContext"/>
	/// <seealso cref="IViewControllerFactory"/>
	/// <seealso cref="IViewController{TResult}"/>
	/// <seealso href="https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93controller"/>
	public interface IViewController : ICommandTarget
	{
		/// <summary>
		/// Gets a view managed by the controller. Should never returns <see langword="null"/>.
		/// </summary>
		IView View { get; }
	}
}
