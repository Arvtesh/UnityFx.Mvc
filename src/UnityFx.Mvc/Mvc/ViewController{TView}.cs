// Copyright (c) Alexander Bogarsukov.
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
	public abstract class ViewController<TView> : ViewController, IViewController<TView> where TView : class, IView
	{
		#region data
		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewController{TView}"/> class.
		/// </summary>
		/// <param name="context">Context data for the controller instance.</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="context"/> is <see langword="null"/>.</exception>
		protected ViewController(IViewControllerContext context)
			: base(context)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewController{TView}"/> class.
		/// </summary>
		/// <param name="context">Context data for the controller instance.</param>
		/// <param name="view">A view managed by the controller.</param>
		/// <exception cref="ArgumentNullException">Thrown if the either <paramref name="context"/> or <paramref name="view"/> is <see langword="null"/>.</exception>
		protected ViewController(IViewControllerContext context, TView view)
			: base(context, view)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewController{TView}"/> class.
		/// </summary>
		/// <param name="context">Context data for the controller instance.</param>
		/// <param name="view">A view managed by the controller.</param>
		/// <param name="viewOptions">View-related flags.</param>
		/// <exception cref="ArgumentNullException">Thrown if the either <paramref name="context"/> or <paramref name="view"/> is <see langword="null"/>.</exception>
		protected ViewController(IViewControllerContext context, TView view, ViewOptions viewOptions)
			: base(context, view, viewOptions)
		{
		}

		#endregion

		#region IViewController

		/// <summary>
		/// Gets a view managed by the controller.
		/// </summary>
		public new TView View
		{
			get
			{
				return (TView)base.View;
			}
		}

		#endregion

		#region implementation
		#endregion
	}
}
