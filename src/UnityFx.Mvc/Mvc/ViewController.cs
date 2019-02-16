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
	public abstract class ViewController : IViewController, IPresentableEvents, IPresenter, ICommandTarget
	{
		#region data

		private enum StateId
		{
			Initialized,
			Presented,
			Active,
			Dismissed,
			Disposed
		}

		private readonly IViewControllerContext _context;

		private ViewOptions _viewOptions;
		private IView _view;
		private StateId _state;

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
		protected bool IsPresented => _state == StateId.Presented || _state == StateId.Active;

		/// <summary>
		/// Gets a value indicating whether the controller has been dismissed.
		/// </summary>
		protected bool IsDismissed => _state == StateId.Dismissed;

		/// <summary>
		/// Gets a value indicating whether the controller is active (i.e. can accept input).
		/// </summary>
		protected bool IsActive => _state == StateId.Active;

		/// <summary>
		/// Gets a value indicating whether the controller is disposed.
		/// </summary>
		protected bool IsDisposed => _state == StateId.Disposed;

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
			_view.Disposed += OnViewDisposed;
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
			_view.Disposed += OnViewDisposed;
			_viewOptions = viewOptions;
		}

		/// <summary>
		/// Sets the view value.
		/// </summary>
		/// <param name="view">The view reference to set.</param>
		/// <param name="viewOptions">View-related flags.</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="view"/> is <see langword="null"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the controller is disposed.</exception>
		/// <seealso cref="RaiseLoadViewCompleted(Exception, bool, object)"/>
		protected void SetView(IView view, ViewOptions viewOptions)
		{
			ThrowIfDisposed();

			if (_view != null)
			{
				_view.Disposed -= OnViewDisposed;
			}

			_view = view ?? throw new ArgumentNullException(nameof(view));
			_viewOptions = viewOptions;

			if (_view != null)
			{
				_view.Disposed += OnViewDisposed;
			}
		}

		/// <summary>
		/// Dissmisses the controller.
		/// </summary>
		protected void Dismiss()
		{
			_context.Dismiss();
		}

		/// <summary>
		/// Raises <see cref="LoadViewCompleted"/> event.
		/// </summary>
		/// <seealso cref="SetView(IView, ViewOptions)"/>
		protected void RaiseLoadViewCompleted(Exception error, bool cancelled, object userState)
		{
			LoadViewCompleted?.Invoke(this, new AsyncCompletedEventArgs(error, cancelled, userState));
		}

		/// <summary>
		/// Throws an <see cref="ObjectDisposedException"/> if the controller is disposed.
		/// </summary>
		/// <seealso cref="Dispose()"/>
		protected void ThrowIfDisposed()
		{
			if (_state == StateId.Disposed)
			{
				throw new ObjectDisposedException(_context.ControllerTypeName);
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
		/// Initiates loading view. Default implementation throws <see cref="NotSupportedException"/>.
		/// </summary>
		protected virtual void OnLoadView()
		{
			throw new NotSupportedException();
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
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="ThrowIfDisposed"/>
		protected virtual void OnDispose()
		{
		}

		#endregion

		#region IViewController

		/// <summary>
		/// Raised when <see cref="LoadView"/> is completed.
		/// </summary>
		/// <seealso cref="View"/>
		/// <seealso cref="LoadView"/>
		public event EventHandler<AsyncCompletedEventArgs> LoadViewCompleted;

		/// <summary>
		/// Raised when <see cref="UnloadView"/> is completed.
		/// </summary>
		/// <seealso cref="View"/>
		/// <seealso cref="UnloadView"/>
		public event EventHandler<AsyncCompletedEventArgs> UnloadViewCompleted;

		/// <summary>
		/// Gets the controller name.
		/// </summary>
		public string Name => _context.ControllerTypeName;

		/// <summary>
		/// Gets a value indicating whether <see cref="View"/> is being loaded/unloaded.
		/// </summary>
		/// <seealso cref="View"/>
		/// <seealso cref="LoadView"/>
		public bool IsBusy => false;

		/// <summary>
		/// Gets a value indicating whether the <see cref="View"/> can be safely used.
		/// </summary>
		/// <seealso cref="View"/>
		/// <seealso cref="LoadView"/>
		public bool IsViewLoaded => _view != null;

		/// <summary>
		/// Gets a view managed by the controller. Returns <see langword="null"/> if the view is not loaded.
		/// </summary>
		/// <seealso cref="IsViewLoaded"/>
		/// <seealso cref="LoadView"/>
		/// <seealso cref="UnloadView"/>
		public IView View => _view;

		/// <summary>
		/// Loads <see cref="View"/>. If view is already loaded (or another load operation is already running) the method returns immediately.
		/// </summary>
		/// <remarks>
		/// Implementation may decide to load views asynchronously. In this case the method just initiates the operation and returns.
		/// Subscribe to <see cref="LoadViewCompleted"/> event to await the load results.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if unload operation is pending.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the controller is disposed.</exception>
		/// <seealso cref="View"/>
		/// <seealso cref="LoadViewCompleted"/>
		/// <seealso cref="UnloadView"/>
		public void LoadView()
		{
			ThrowIfDisposed();

			if (_view == null)
			{
				OnLoadView();
			}
		}

		/// <summary>
		/// Unloads the view. Does nothing is view is not loaded or another unload operation is already running. Cancels load operation (if any).
		/// </summary>
		/// <remarks>
		/// Implementation may decide to unload views asynchronously. In this case the method just initiates the operation and returns.
		/// Subscribe to <see cref="UnloadViewCompleted"/> event to await the unload results.
		/// </remarks>
		/// <seealso cref="View"/>
		/// <seealso cref="UnloadViewCompleted"/>
		/// <seealso cref="LoadView"/>
		public void UnloadView()
		{
			if ((_viewOptions & ViewOptions.DoNotDispose) == 0)
			{
				_view?.Dispose();
				_view = null;
			}
		}

		#endregion

		#region IPresentableEvents

		/// <inheritdoc/>
		void IPresentableEvents.OnPresent()
		{
			Debug.Assert(_state == StateId.Initialized);

			OnPresent();

			_view.Command += OnCommand;
			_state = StateId.Presented;
		}

		/// <inheritdoc/>
		void IPresentableEvents.OnDismiss()
		{
			Debug.Assert(_state == StateId.Presented);

			_state = StateId.Dismissed;
			_view.Command -= OnCommand;

			OnDismiss();
		}

		/// <inheritdoc/>
		void IPresentableEvents.OnActivate()
		{
			Debug.Assert(_state == StateId.Presented);

			_state = StateId.Active;

			OnActivate();
		}

		/// <inheritdoc/>
		void IPresentableEvents.OnDeactivate()
		{
			Debug.Assert(_state == StateId.Active);

			try
			{
				OnDeactivate();
			}
			finally
			{
				_state = StateId.Presented;
			}
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
		public IPresentResult<TController> Present<TController>() where TController : class, IViewController
		{
			ThrowIfDisposed();
			return _context.Present<TController>(PresentArgs.Default);
		}

		/// <inheritdoc/>
		public IPresentResult<TController> Present<TController>(PresentArgs args) where TController : class, IViewController
		{
			ThrowIfDisposed();
			return _context.Present<TController>(args);
		}

		#endregion

		#region ICommandTarget

		/// <summary>
		/// Invokes a command.
		/// </summary>
		/// <param name="commandName">Name of the command to invoke.</param>
		/// <param name="args">Command-specific arguments.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="commandName"/> is <see langword="null"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the controller is disposed.</exception>
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
		/// <seealso cref="ThrowIfDisposed"/>
		public void Dispose()
		{
			if (_state != StateId.Disposed)
			{
				var needCallDismiss = _state == StateId.Active && _state == StateId.Presented;

				_state = StateId.Disposed;

				try
				{
					if (needCallDismiss)
					{
						_context.Dismiss();
					}

					OnDispose();
				}
				finally
				{
					if ((_viewOptions & ViewOptions.DoNotDispose) == 0)
					{
						_view?.Dispose();
						_view = null;
					}
				}
			}
		}

		#endregion

		#region implementation

		private void OnViewDisposed(object sender, EventArgs args)
		{
			_view = null;
		}

		private void OnCommand(object sender, CommandEventArgs e)
		{
			Debug.Assert(e != null);
			Debug.Assert(_state != StateId.Disposed);

			OnCommand(e);
		}

		#endregion
	}
}
