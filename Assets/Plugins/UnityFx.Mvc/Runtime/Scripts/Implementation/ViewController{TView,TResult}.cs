// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A generic view controller bound to a view and result type. It is recommended to use this class as base for all other controllers.
	/// Note that minimal controller implementation should implement <see cref="IViewController"/>.
	/// </summary>
	/// <seealso cref="ViewController"/>
	public abstract class ViewController<TView, TResult> : ViewController<TView>, IViewController<TResult> where TView : class, IView
	{
		#region data

		private TResult _result;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewController{TView}"/> class.
		/// </summary>
		/// <param name="view">A view managed by the controller.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="view"/> is <see langword="null"/>.</exception>
		protected ViewController(TView view)
			: base(view)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewController{TView}"/> class.
		/// </summary>
		/// <param name="view">A view managed by the controller.</param>
		/// <param name="viewOptions">View-related flags.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="view"/> is <see langword="null"/>.</exception>
		protected ViewController(TView view, ViewOptions viewOptions)
			: base(view, viewOptions)
		{
		}

		/// <summary>
		/// Dismissed the controller with a result value.
		/// </summary>
		protected void Dismiss(TResult result)
		{
			_result = result;
			Dismiss();
		}

		#endregion

		#region IViewController

		/// <summary>
		/// Gets a controller result value (if any).
		/// </summary>
		public TResult Result { get => _result; protected set => _result = value; }

		#endregion

		#region implementation
		#endregion
	}
}
