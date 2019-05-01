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
		private bool _dismissed;

		#endregion

		#region interface

		/// <summary>
		/// Enumerates basic controller comaands.
		/// </summary>
		public abstract class Commands
		{
			#region Common

			/// <summary>
			/// Name of the OK command.
			/// </summary>
			public const string Ok = "Ok";

			/// <summary>
			/// Name of the CANCEL command.
			/// </summary>
			public const string Cancel = "Cancel";

			/// <summary>
			/// Name of the APPLY command.
			/// </summary>
			public const string Apply = "Apply";

			/// <summary>
			/// Name of the EXIT command.
			/// </summary>
			public const string Exit = "Exit";

			/// <summary>
			/// Name of the HELP command.
			/// </summary>
			public const string Help = "Help";

			/// <summary>
			/// Name of the ADD command.
			/// </summary>
			public const string Add = "Add";

			/// <summary>
			/// Name of the REMOVE command.
			/// </summary>
			public const string Remove = "Remove";

			#endregion

			#region Navigation

			/// <summary>
			/// Name of the BACK command.
			/// </summary>
			public const string Back = "Back";

			/// <summary>
			/// Name of the NEXT command.
			/// </summary>
			public const string Next = "Next";

			/// <summary>
			/// Name of the PREV command.
			/// </summary>
			public const string Prev = "Prev";

			#endregion

			#region File

			/// <summary>
			/// Name of the NEW command.
			/// </summary>
			public const string New = "New";

			/// <summary>
			/// Name of the OPEN command.
			/// </summary>
			public const string Open = "Open";

			/// <summary>
			/// Name of the CLOSE command.
			/// </summary>
			public const string Close = "Close";

			/// <summary>
			/// Name of the SAVE command.
			/// </summary>
			public const string Save = "Save";

			#endregion

			#region Editing

			/// <summary>
			/// Name of the EDIT command.
			/// </summary>
			public const string Edit = "Edit";

			/// <summary>
			/// Name of the COPY command.
			/// </summary>
			public const string Copy = "Copy";

			/// <summary>
			/// Name of the CUT command.
			/// </summary>
			public const string Cut = "Cut";

			/// <summary>
			/// Name of the PASTE command.
			/// </summary>
			public const string Paste = "Paste";

			/// <summary>
			/// Name of the DUPLICATE command.
			/// </summary>
			public const string Duplicate = "Duplicate";

			/// <summary>
			/// Name of the DELETE command.
			/// </summary>
			public const string Delete = "Delete";

			/// <summary>
			/// Name of the UNDO command.
			/// </summary>
			public const string Undo = "Undo";

			/// <summary>
			/// Name of the REDO command.
			/// </summary>
			public const string Redo = "Redo";

			#endregion
		}

		/// <summary>
		/// Gets a value indicating whether the controller is active (i.e. can accept input).
		/// </summary>
		protected bool IsActive => _context.IsActive;

		/// <summary>
		/// Gets a value indicating whether the controller is presented.
		/// </summary>
		protected bool IsPresented => _presented;

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
		/// Called right after the controller has been presented. Default implementation does nothing.
		/// </summary>
		/// <seealso cref="OnDismiss"/>
		protected virtual void OnPresented()
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
		/// Called when the controller is being dismissed. Default implementation disposes the controller.
		/// </summary>
		/// <seealso cref="Dismiss"/>
		protected virtual void OnDismiss()
		{
			Dispose();
		}

		#endregion

		#region ViewController

		/// <inheritdoc/>
		protected override void OnLoadViewCompleted(AsyncCompletedEventArgs args)
		{
			if (args.Error == null && !args.Cancelled)
			{
				if (!_dismissed)
				{
					_presented = true;
					OnPresented();
				}
			}

			base.OnLoadViewCompleted(args);
		}

		/// <inheritdoc/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (!_dismissed)
				{
					_dismissed = true;
					Dismissed?.Invoke(this, EventArgs.Empty);
				}
			}

			base.Dispose(disposing);
		}

		#endregion

		#region IPresentable
		#endregion

		#region IPresentableEvents

		/// <inheritdoc/>
		void IPresentableEvents.OnActivate()
		{
			Debug.Assert(_presented);
			Debug.Assert(!_dismissed);
			OnActivate();
		}

		/// <inheritdoc/>
		void IPresentableEvents.OnDeactivate()
		{
			Debug.Assert(_presented);
			Debug.Assert(!_dismissed);
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

		/// <inheritdoc/>
		public event EventHandler Dismissed;

		/// <inheritdoc/>
		public bool IsDismissed => _dismissed;

		/// <inheritdoc/>
		public void Dismiss()
		{
			if (_presented)
			{
				if (!_dismissed)
				{
					_dismissed = true;
					Dismissed?.Invoke(this, EventArgs.Empty);
					OnDismiss();
				}
			}
			else
			{
				_dismissed = true;
				Dispose();
			}
		}

		#endregion

		#region implementation
		#endregion
	}
}
