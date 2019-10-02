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
		/// Initializes a new instance of the <see cref="ViewController{TView, TResult}"/> class.
		/// </summary>
		/// <param name="context">A controller context.</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="context"/> is <see langword="null"/>.</exception>
		protected ViewController(IPresentContext context)
			: base(context)
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
