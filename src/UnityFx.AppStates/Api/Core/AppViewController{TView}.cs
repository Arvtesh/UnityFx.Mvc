// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic view controller bound to a specific view component. It is recommended to use this class as base for all other controllers.
	/// Note that minimal controller implementation should inherit <see cref="IPresentable"/>.
	/// </summary>
	public class AppViewController<TView> : AppViewController, IPresentable<TView> where TView : class
	{
		#region data

		private TView _viewAspect;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="AppViewController{TView}"/> class.
		/// </summary>
		/// <param name="context">Context data for the controller instance.</param>
		protected AppViewController(IPresentableContext context)
			: base(context)
		{
		}

		#endregion

		#region IPresentable

		/// <summary>
		/// Gets the <typeparamref name="TView"/> view component.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the view is not loaded.</exception>
		public TView ViewAspect
		{
			get
			{
				if (_viewAspect != null)
				{
					return _viewAspect;
				}
				else if (View.IsLoaded)
				{
					_viewAspect = View.GetComponent<TView>() ?? throw new InvalidOperationException();
					return _viewAspect;
				}
				else
				{
					throw new InvalidOperationException();
				}
			}
		}

		#endregion
	}
}
