// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic view controller bound to a view. It is recommended to use this class as base for all other controllers.
	/// Note that minimal controller implementation should implement <see cref="IViewController"/>.
	/// </summary>
	/// <seealso cref="ViewController"/>
	public abstract class ViewController<TView> : ViewController, IViewController<TView> where TView : class, IView
	{
		#region data

		private TView _view;

		#endregion

		#region interface

		/// <summary>
		/// Gets a value indicating whether view is loaded.
		/// </summary>
		protected bool IsViewLoaded => _view != null;

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
		/// Called when the view is loaded. Default implementation does nothing.
		/// </summary>
		protected virtual void OnViewLoaded()
		{
		}

		#endregion

		#region ViewController

		/// <summary>
		/// Performs any asynchronous actions needed to present this object. The method is invoked by the system.
		/// </summary>
		/// <param name="presentContext">Context data provided by the system.</param>
		/// <returns>Returns an object that can be used to track the operation state.</returns>
		protected override IAsyncOperation PresentAsync(IPresentContext presentContext)
		{
			if (_view != null)
			{
				throw new InvalidOperationException();
			}

			return LoadView().ContinueWith(OnViewLoadedInternal, this);
		}

		/// <summary>
		/// Releases resources used by the controller.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_view?.Dispose();
			}
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
