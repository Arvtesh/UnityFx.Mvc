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
	/// Note that minimal controller implementation should inherit <see cref="IViewController"/>.
	/// </summary>
	public abstract class ViewController<TView> : ViewController where TView : class
	{
		#region data

		private TView _viewAspect;

		#endregion

		#region interface

		/// <summary>
		/// Gets the <typeparamref name="TView"/> view component. The componet reference is cached on first access. Accessing this property before view is loaded throws <see cref="InvalidOperationException"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the view is not loaded or the <typeparamref name="TView"/> component is not attached to the view.</exception>
		protected TView ViewAspect
		{
			get
			{
				if (_viewAspect != null)
				{
					return _viewAspect;
				}
				else
				{
					_viewAspect = View.GetComponent<TView>();

					if (_viewAspect == null)
					{
						throw new InvalidOperationException();
					}

					return _viewAspect;
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewController{TView}"/> class.
		/// </summary>
		/// <param name="context">Context data for the controller instance.</param>
		protected ViewController(PresentContext context)
			: base(context)
		{
		}

		#endregion
	}
}
