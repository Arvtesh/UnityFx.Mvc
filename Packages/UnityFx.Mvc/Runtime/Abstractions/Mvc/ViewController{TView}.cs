// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A generic view controller bound to a view. It is recommended to use this class as base for all other controllers.
	/// Note that minimal controller implementation should implement <see cref="IViewController"/>.
	/// </summary>
	/// <seealso cref="ViewController"/>
	public abstract class ViewController<TView> : ViewController, IViewControllerView<TView> where TView : class, IView
	{
		#region data
		#endregion

		#region interface

		/// <summary>
		/// Gets a view managed by the controller. Never returns <see langword="null"/>.
		/// </summary>
		protected new TView View => (TView)base.View;

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewController{TView}"/> class.
		/// </summary>
		/// <param name="context">A controller context.</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="context"/> is <see langword="null"/>.</exception>
		protected ViewController(IPresentContext context)
			: base(context)
		{
		}

		#endregion

		#region implementation
		#endregion
	}
}
