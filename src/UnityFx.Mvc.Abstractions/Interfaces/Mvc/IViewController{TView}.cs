// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A generic view controller.
	/// </summary>
	/// <seealso cref="IView"/>
	/// <seealso cref="IPresenter"/>
	/// <seealso cref="IPresentService"/>
	/// <seealso cref="IViewControllerContext"/>
	/// <seealso cref="IViewControllerFactory"/>
	/// <seealso cref="IViewController"/>
	public interface IViewController<TView> : IViewController where TView : class, IView
	{
		/// <summary>
		/// Gets a view managed by the controller. Returns <see langword="null"/> if the view is not loaded.
		/// </summary>
		/// <remarks>
		/// Implementation may decide to lazy-load its view on first access. In this case the property would never return <see langword="null"/>.
		/// </remarks>
		new TView View { get; }
	}
}
