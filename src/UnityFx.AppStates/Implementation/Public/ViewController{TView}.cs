// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic view controller bound to a view. It is recommended to use this class as base for all other controllers.
	/// Note that minimal controller implementation should implement <see cref="IViewController"/>.
	/// </summary>
	public abstract class ViewController<TView> : ViewController, IViewController<TView> where TView : class, IView
	{
		#region data

		private TView _view;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewController{TView}"/> class.
		/// </summary>
		/// <param name="context">Context data for the controller instance.</param>
		protected ViewController(IViewControllerContext context)
			: base(context)
		{
		}

		/// <summary>
		/// Loads the controller view.
		/// </summary>
		protected abstract IAsyncOperation<TView> LoadView();

		/// <summary>
		/// Called when the view is loaded.
		/// </summary>
		protected virtual void OnViewLoaded()
		{
		}

		#endregion

		#region ViewController

		/// <summary>
		/// Performs any asynchronous actions needed to present this object. The method is invoked by the system.
		/// </summary>
		public override IAsyncOperation PresentAsync(IPresentContext presentContext)
		{
			return LoadView().ContinueWith(OnViewLoadedInternal, this);
		}

		#endregion

		#region IViewController

		/// <summary>
		/// Gets a view managed by the controller.
		/// </summary>
		public TView View
		{
			get
			{
				if (_view == null)
				{
					throw new InvalidOperationException();
				}

				return _view;
			}
		}

		#endregion

		#region implementation

		private static void OnViewLoadedInternal(IAsyncOperation<TView> op, object userState)
		{
			var controller = (ViewController<TView>)userState;

			if (op.IsCompletedSuccessfully)
			{
				controller._view = op.Result;
				controller.OnViewLoaded();
			}
		}

		#endregion
	}
}
