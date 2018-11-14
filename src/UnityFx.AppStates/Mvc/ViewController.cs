﻿// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using UnityFx.Async;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A generic view controller. It is recommended to use this class as base for all other controllers.
	/// Note that minimal controller implementation should implement <see cref="IViewController"/>.
	/// </summary>
	/// <seealso cref="ViewController{TView}"/>
	public abstract class ViewController : IViewController, IPresentableEvents, IPresenter, ICommandTarget
	{
		#region data

		private readonly IViewControllerContext _context;

		private ViewOptions _viewOptions;
		private IView _view;

		private IAsyncOperation _dismissOp;

		private bool _presented;
		private bool _active;
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Defines view-related flags.
		/// </summary>
		[Flags]
		public enum ViewOptions
		{
			/// <summary>
			/// No options.
			/// </summary>
			None = 0,

			/// <summary>
			/// If set, the view managed by the controller is not disposed.
			/// </summary>
			DoNotDispose = 1
		}

		/// <summary>
		/// Enumerates basic controller comaands.
		/// </summary>
		public abstract class Commands
		{
			/// <summary>
			/// Name of the OK command.
			/// </summary>
			public const string Ok = "Ok";

			/// <summary>
			/// Name of the CANCEL command.
			/// </summary>
			public const string Cancel = "Cancel";

			/// <summary>
			/// Name of the BACK command.
			/// </summary>
			public const string Back = "Back";

			/// <summary>
			/// Name of the APPLY command.
			/// </summary>
			public const string Apply = "Apply";

			/// <summary>
			/// Name of the NEXT command.
			/// </summary>
			public const string Next = "Next";

			/// <summary>
			/// Name of the PREV command.
			/// </summary>
			public const string Prev = "Prev";

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

			/// <summary>
			/// Name of the EXIT command.
			/// </summary>
			public const string Exit = "Exit";

			/// <summary>
			/// Name of the ADD command.
			/// </summary>
			public const string Add = "Add";

			/// <summary>
			/// Name of the REMOVE command.
			/// </summary>
			public const string Remove = "Remove";

			/// <summary>
			/// Name of the EDIT command.
			/// </summary>
			public const string Edit = "Edit";

			/// <summary>
			/// Name of the HELP command.
			/// </summary>
			public const string Help = "Help";

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
		}

		/// <summary>
		/// Gets a value indicating whether the controller is presented.
		/// </summary>
		protected bool IsPresented => _presented;

		/// <summary>
		/// Gets a value indicating whether the controller is active (i.e. can accept input).
		/// </summary>
		protected bool IsActive => _active;

		/// <summary>
		/// Gets a value indicating whether the controller is disposed.
		/// </summary>
		protected bool IsDisposed => _disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewController"/> class.
		/// </summary>
		/// <param name="context">The controller context.</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="context"/> is <see langword="null"/>.</exception>
		protected ViewController(IViewControllerContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewController"/> class.
		/// </summary>
		/// <param name="context">The controller context.</param>
		/// <param name="view">A view managed by the controller.</param>
		/// <exception cref="ArgumentNullException">Thrown if the either <paramref name="context"/> or <paramref name="view"/> is <see langword="null"/>.</exception>
		protected ViewController(IViewControllerContext context, IView view)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_view = view ?? throw new ArgumentNullException(nameof(view));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewController"/> class.
		/// </summary>
		/// <param name="context">The controller context.</param>
		/// <param name="view">A view managed by the controller.</param>
		/// <param name="viewOptions">View-related flags.</param>
		/// <exception cref="ArgumentNullException">Thrown if the either <paramref name="context"/> or <paramref name="view"/> is <see langword="null"/>.</exception>
		protected ViewController(IViewControllerContext context, IView view, ViewOptions viewOptions)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_view = view ?? throw new ArgumentNullException(nameof(view));
			_viewOptions = viewOptions;
		}

		/// <summary>
		/// Sets the view value.
		/// </summary>
		/// <param name="view">The view reference to set.</param>
		/// <param name="viewOptions">View-related flags.</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="view"/> is <see langword="null"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the controller is disposed.</exception>
		protected void SetView(IView view, ViewOptions viewOptions)
		{
			ThrowIfDisposed();

			_view = view ?? throw new ArgumentNullException(nameof(view));
			_viewOptions = viewOptions;
		}

		/// <summary>
		/// Throws an <see cref="ObjectDisposedException"/> if the controller is disposed.
		/// </summary>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="Dispose(bool)"/>
		protected void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(_context.Name);
			}
		}

		/// <summary>
		/// Called to process a command.
		/// </summary>
		/// <param name="e">Command name and arguments.</param>
		/// <returns>Returns <see langword="true"/> if the command has been handles; <see langword="false"/> otherwise.</returns>
		protected virtual bool OnCommand(CommandEventArgs e)
		{
			return false;
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

		/// <summary>
		/// Called when the controller is disposed. Default implementation does nothing.
		/// </summary>
		/// <seealso cref="Dispose(bool)"/>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="ThrowIfDisposed"/>
		protected virtual void OnDispose()
		{
		}

		/// <summary>
		/// Releases resources used by the controller.
		/// </summary>
		/// <param name="disposing">Should be <see langword="true"/> if the method is called from <see cref="Dispose()"/>; <see langword="false"/> otherwise.</param>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="OnDispose"/>
		/// <seealso cref="ThrowIfDisposed"/>
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				_disposed = true;

				if (disposing)
				{
					try
					{
						OnDispose();
					}
					finally
					{
						if ((_viewOptions & ViewOptions.DoNotDispose) == 0)
						{
							_view?.Dispose();
						}
					}
				}
			}
		}

		#endregion

		#region IViewController

		/// <summary>
		/// Gets the controller name.
		/// </summary>
		public string Name => _context.Name;

		/// <summary>
		/// Gets a value indicating whether the <see cref="View"/> can be safely used.
		/// </summary>
		/// <seealso cref="View"/>
		public bool IsViewLoaded => _view != null;

		/// <summary>
		/// Gets a view managed by the controller.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the view is not initialized.</exception>
		/// <seealso cref="IsViewLoaded"/>
		public IView View => _view ?? throw new InvalidOperationException();

		#endregion

		#region IPresentableEvents

		/// <inheritdoc/>
		void IPresentableEvents.OnPresent()
		{
			Debug.Assert(!_disposed);
			Debug.Assert(!_active);
			Debug.Assert(!_presented);

			OnPresent();

			_view.Command += OnCommand;
			_presented = true;
		}

		/// <inheritdoc/>
		void IPresentableEvents.OnDismiss()
		{
			Debug.Assert(!_disposed);
			Debug.Assert(!_active);
			Debug.Assert(_presented);

			_presented = false;
			_view.Command -= OnCommand;

			OnDismiss();
		}

		/// <inheritdoc/>
		void IPresentableEvents.OnActivate()
		{
			Debug.Assert(!_disposed);
			Debug.Assert(!_active);
			Debug.Assert(_presented);

			_active = true;

			OnActivate();
		}

		/// <inheritdoc/>
		void IPresentableEvents.OnDeactivate()
		{
			Debug.Assert(!_disposed);
			Debug.Assert(_active);
			Debug.Assert(_presented);

			try
			{
				OnDeactivate();
			}
			finally
			{
				_active = false;
			}
		}

		#endregion

		#region IPresenter

		/// <inheritdoc/>
		public IAsyncOperation<IViewController> PresentAsync(Type controllerType)
		{
			ThrowIfDisposed();
			return _context.PresentAsync(controllerType, PresentArgs.Default);
		}

		/// <inheritdoc/>
		public IAsyncOperation<IViewController> PresentAsync(Type controllerType, PresentArgs args)
		{
			ThrowIfDisposed();
			return _context.PresentAsync(controllerType, args);
		}

		/// <inheritdoc/>
		public IAsyncOperation<TController> PresentAsync<TController>() where TController : class, IViewController
		{
			ThrowIfDisposed();
			return _context.PresentAsync<TController>(PresentArgs.Default);
		}

		/// <inheritdoc/>
		public IAsyncOperation<TController> PresentAsync<TController>(PresentArgs args) where TController : class, IViewController
		{
			ThrowIfDisposed();
			return _context.PresentAsync<TController>(args);
		}

		#endregion

		#region IDismissable

		/// <inheritdoc/>
		public IAsyncOperation DismissAsync()
		{
			if (_dismissOp == null)
			{
				if (_disposed)
				{
					_dismissOp = AsyncResult.CompletedOperation;
				}
				else
				{
					_dismissOp = _context.DismissAsync();
				}
			}

			return _dismissOp;
		}

		#endregion

		#region ICommandTarget

		/// <summary>
		/// Invokes a command.
		/// </summary>
		/// <param name="commandName">Name of the command to invoke.</param>
		/// <param name="args">Command-specific arguments.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="commandName"/> is <see langword="null"/>.</exception>
		/// <returns>Returns <see langword="true"/> if the command has been handles; <see langword="false"/> otherwise.</returns>
		public bool InvokeCommand(string commandName, object args)
		{
			ThrowIfDisposed();

			if (commandName == null)
			{
				throw new ArgumentNullException(nameof(commandName));
			}

			return OnCommand(new CommandEventArgs(commandName, args));
		}

		#endregion

		#region IDisposable

		/// <summary>
		/// Releases resources used by the controller.
		/// </summary>
		/// <remarks>
		/// This method is called by parent state. It should not be called by user code.
		/// </remarks>
		/// <seealso cref="Dispose(bool)"/>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		#region implementation

		private void OnCommand(object sender, CommandEventArgs e)
		{
			Debug.Assert(e != null);
			Debug.Assert(!_disposed);

			OnCommand(e);
		}

		#endregion
	}
}
