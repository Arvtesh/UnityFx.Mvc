// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Implementation of <see cref="IViewController"/>.
	/// </summary>
	/// <seealso cref="ViewController{TView}"/>
	public abstract class ViewController : IViewController, IViewControllerEvents
	{
		#region data

		private readonly IView _view;
		private readonly ViewOptions _viewOptions;

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
		/// Gets a value indicating whether the controller is disposed.
		/// </summary>
		protected bool IsDisposed => _disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewController"/> class.
		/// </summary>
		/// <param name="view">A view managed by the controller.</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="view"/> is <see langword="null"/>.</exception>
		protected ViewController(IView view)
		{
			_view = view ?? throw new ArgumentNullException(nameof(view));
			_view.Command += OnCommand;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewController"/> class.
		/// </summary>
		/// <param name="view">A view managed by the controller.</param>
		/// <param name="viewOptions">View-related flags.</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="view"/> is <see langword="null"/>.</exception>
		protected ViewController(IView view, ViewOptions viewOptions)
		{
			_view = view ?? throw new ArgumentNullException(nameof(view));
			_view.Command += OnCommand;
			_viewOptions = viewOptions;
		}

		/// <summary>
		/// Throws an <see cref="ObjectDisposedException"/> if the controller is disposed.
		/// </summary>
		/// <seealso cref="Dispose()"/>
		protected void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		/// <summary>
		/// Called to process a command.
		/// </summary>
		/// <returns>Returns <see langword="true"/> if the command has been handles; <see langword="false"/> otherwise.</returns>
		protected virtual bool OnCommand(CommandEventArgs e)
		{
			return false;
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
		/// Called when the controller is being disposed. Should not throw exceptions. Default implementation invokes <see cref="Disposed"/> event.
		/// </summary>
		/// <seealso cref="Dispose"/>
		/// <seealso cref="ThrowIfDisposed"/>
		protected virtual void OnDispose()
		{
			Disposed?.Invoke(this, EventArgs.Empty);
		}

		#endregion

		#region IViewController

		/// <summary>
		/// Raised when the controller is disposed.
		/// </summary>
		public event EventHandler Disposed;

		/// <summary>
		/// Gets a view managed by the controller. Never returns <see langword="null"/>.
		/// </summary>
		public IView View => _view;

		#endregion

		#region IViewControllerEvents

		/// <inheritdoc/>
		void IViewControllerEvents.OnActivate()
		{
			Debug.Assert(!_disposed);
			OnActivate();
		}

		/// <inheritdoc/>
		void IViewControllerEvents.OnDeactivate()
		{
			Debug.Assert(!_disposed);
			OnDeactivate();
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
		/// <seealso cref="OnDispose"/>
		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;

				try
				{
					OnDispose();
				}
				finally
				{
					if (!_viewOptions.HasFlag(ViewOptions.DoNotDispose))
					{
						_view.Dispose();
					}
				}
			}
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
