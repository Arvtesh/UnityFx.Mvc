// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A generic view controller.
	/// </summary>
	/// <seealso cref="IViewController"/>
	/// <seealso cref="IView"/>
	public interface IViewController<TView> : IViewController where TView : class, IView
	{
		/// <summary>
		/// Gets a view managed by the controller.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the view is not initialized.</exception>
		new TView View { get; }
	}
}
