// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A generic view controller. It is recommended to use this class as base for all other controllers.
	/// Note that minimal controller implementation should implement <see cref="IViewController"/>.
	/// </summary>
	/// <seealso cref="ViewController{TView}"/>
	public abstract class Presentable : ViewController, IPresentable, IPresentableEvents, IPresenter
	{
		#region data

		private readonly IPresentContext _context;
		private bool _presented;

		#endregion

		#region interface

		/// <summary>
		/// Gets a value indicating whether the controller is presented.
		/// </summary>
		protected bool IsPresented => _presented;

		/// <summary>
		/// Gets a value indicating whether the controller is active (i.e. can accept input).
		/// </summary>
		protected bool IsActive => _context.IsActive;

		/// <summary>
		/// Initializes a new instance of the <see cref="Presentable"/> class.
		/// </summary>
		/// <param name="context">The controller context.</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="context"/> is <see langword="null"/>.</exception>
		protected Presentable(IPresentContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Presentable"/> class.
		/// </summary>
		/// <param name="context">The controller context.</param>
		/// <param name="view">A view managed by the controller.</param>
		/// <exception cref="ArgumentNullException">Thrown if the either <paramref name="context"/> or <paramref name="view"/> is <see langword="null"/>.</exception>
		protected Presentable(IPresentContext context, IView view)
			: base(view)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Presentable"/> class.
		/// </summary>
		/// <param name="context">The controller context.</param>
		/// <param name="view">A view managed by the controller.</param>
		/// <param name="viewOptions">View-related flags.</param>
		/// <exception cref="ArgumentNullException">Thrown if the either <paramref name="context"/> or <paramref name="view"/> is <see langword="null"/>.</exception>
		protected Presentable(IPresentContext context, IView view, ViewOptions viewOptions)
			: base(view, viewOptions)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Called right after the controller transition animation finishes. Default implementation does nothing.
		/// </summary>
		/// <seealso cref="OnDismiss"/>
		protected virtual void OnPresent()
		{
		}

		/// <summary>
		/// Called right before the controller becomes active. Default implementation does nothing.
		/// </summary>
		/// <seealso cref="OnDeactivate"/>
		protected virtual void OnActivate()
		{
		}

		/// <summary>
		/// Called when the controller is about to become inactive. Default implementation does nothing.
		/// </summary>
		/// <seealso cref="OnActivate"/>
		protected virtual void OnDeactivate()
		{
		}

		/// <summary>
		/// Called when the controller is about to be dismissed (before transition animation). Default implementation does nothing.
		/// </summary>
		/// <seealso cref="OnPresent"/>
		protected virtual void OnDismiss()
		{
		}

		#endregion

		#region ViewController

		/// <summary>
		/// Called when the controller is disposed. Default implementation unloads the attached view.
		/// </summary>
		protected override void OnDispose()
		{
			// Make sure the object is dismissed.
			_context.Dismiss();

			base.OnDispose();
		}

		#endregion

		#region IPresentable
		#endregion

		#region IPresentableEvents

		/// <inheritdoc/>
		void IPresentableEvents.OnPresent()
		{
			Debug.Assert(!IsDismissed);
			OnPresent();
		}

		/// <inheritdoc/>
		void IPresentableEvents.OnDismiss()
		{
			Debug.Assert(!IsDisposed);
			OnDismiss();
			Dismissed?.Invoke(this, EventArgs.Empty);
		}

		/// <inheritdoc/>
		void IPresentableEvents.OnActivate()
		{
			Debug.Assert(!IsDismissed);
			OnActivate();
		}

		/// <inheritdoc/>
		void IPresentableEvents.OnDeactivate()
		{
			Debug.Assert(!IsDismissed);
			OnDeactivate();
		}

		#endregion

		#region IPresenter

		/// <inheritdoc/>
		public IPresentResult Present(Type controllerType)
		{
			ThrowIfDisposed();
			return _context.Present(controllerType, PresentArgs.Default);
		}

		/// <inheritdoc/>
		public IPresentResult Present(Type controllerType, PresentArgs args)
		{
			ThrowIfDisposed();
			return _context.Present(controllerType, args);
		}

		/// <inheritdoc/>
		public IPresentResult<TController> Present<TController>() where TController : class, IPresentable
		{
			ThrowIfDisposed();
			return _context.Present<TController>(PresentArgs.Default);
		}

		/// <inheritdoc/>
		public IPresentResult<TController> Present<TController>(PresentArgs args) where TController : class, IPresentable
		{
			ThrowIfDisposed();
			return _context.Present<TController>(args);
		}

		#endregion

		#region IDismissable

		/// <summary>
		/// Raised when the instance is dismissed.
		/// </summary>
		/// <seealso cref="Dismiss"/>
		/// <seealso cref="IsDismissed"/>
		public event EventHandler Dismissed;

		/// <summary>
		/// Gets a value indicating whether the object is dismissed.
		/// </summary>
		/// <seealso cref="Dismiss"/>
		public bool IsDismissed => IsDisposed;

		/// <summary>
		/// Dismisses the obejct.
		/// </summary>
		/// <seealso cref="IsDismissed"/>
		public void Dismiss()
		{
			_context.Dismiss();
		}

		#endregion

		#region implementation
		#endregion
	}
}
