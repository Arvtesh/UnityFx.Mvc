// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic view controller bound to a view. It is recommended to use this class as base for all other controllers.
	/// Note that minimal controller implementation should implement <see cref="IViewController"/>.
	/// </summary>
	/// <seealso cref="ViewController"/>
	public abstract class ViewController<TView> : ViewController where TView : class, IView
	{
		#region data
		#endregion

		#region interface

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

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewController{TView}"/> class.
		/// </summary>
		/// <param name="context">Context data for the controller instance.</param>
		protected ViewController(IViewControllerContext context)
			: base(context)
		{
		}

		#endregion

		#region ViewController
		#endregion

		#region implementation
		#endregion
	}
}
