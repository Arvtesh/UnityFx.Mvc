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
	/// <seealso cref="IPresentContext"/>
	/// <seealso cref="IPresentService"/>
	/// <seealso cref="IViewControllerFactory"/>
	/// <seealso cref="IViewController"/>
	public interface IViewController<TView> : IViewController where TView : class, IView
	{
		/// <summary>
		/// Gets a view managed by the controller. Never returns <see langword="null"/>.
		/// </summary>
		new TView View { get; }
	}
}
