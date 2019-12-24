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

		private readonly IPresentContext _context;

		#endregion

		#region interface

		/// <summary>
		/// Enumerates basic controller commands.
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
			/// Name of the ABOUT command.
			/// </summary>
			public const string About = "About";

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

			/// <summary>
			/// Name of the HOME command.
			/// </summary>
			public const string Home = "Home";

			/// <summary>
			/// Name of the END command.
			/// </summary>
			public const string End = "End";

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
			/// Name of the INSERT command.
			/// </summary>
			public const string Insert = "Insert";

			/// <summary>
			/// Name of the DELETE command.
			/// </summary>
			public const string Delete = "Delete";

			/// <summary>
			/// Name of the DUPLICATE command.
			/// </summary>
			public const string Duplicate = "Duplicate";

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
		/// Gets the controller context.
		/// </summary>
		protected IPresentContext Context => _context;

		/// <summary>
		/// Gets a value indicating whether the controller is dismissed.
		/// </summary>
		/// <seealso cref="Dismiss"/>
		protected bool IsDismissed => _context.IsDismissed;

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewController"/> class.
		/// </summary>
		/// <param name="context">A controller context.</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="context"/> is <see langword="null"/>.</exception>
		protected ViewController(IPresentContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_context.View.Command += OnCommand;
		}

		/// <summary>
		/// Dismissses this controller.
		/// </summary>
		/// <seealso cref="IsDismissed"/>
		protected void Dismiss()
		{
			_context.Dismiss();
		}

		/// <summary>
		/// Throws an <see cref="ObjectDisposedException"/> if the controller is disposed.
		/// </summary>
		/// <seealso cref="Dispose()"/>
		protected void ThrowIfDismissed()
		{
			if (_context.IsDismissed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		/// <summary>
		/// Called to process a command.
		/// </summary>
		/// <returns>Returns <see langword="true"/> if the command has been handles; <see langword="false"/> otherwise.</returns>
		protected virtual bool OnCommand(string commandName, object commandArgs)
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
		/// Called when the controller has been presented. Default implementation does nothing.
		/// </summary>
		/// <seealso cref="OnDismiss"/>
		protected virtual void OnPresent()
		{
		}

		/// <summary>
		/// Called when the controller is being dismissed. Should not throw exceptions. Default implementation does nothing.
		/// </summary>
		/// <seealso cref="OnPresent"/>
		/// <seealso cref="ThrowIfDismissed"/>
		protected virtual void OnDismiss()
		{
		}

		/// <summary>
		/// Called on each frame. Default implementation does nothing.
		/// </summary>
		protected virtual void OnUpdate(float frameTime)
		{
		}

		#endregion

		#region IViewController

		/// <summary>
		/// Gets a view managed by the controller. Never returns <see langword="null"/>.
		/// </summary>
		public IView View => _context.View;

		#endregion

		#region IViewControllerEvents

		/// <inheritdoc/>
		void IViewControllerEvents.OnActivate()
		{
			Debug.Assert(!IsDismissed);
			OnActivate();
		}

		/// <inheritdoc/>
		void IViewControllerEvents.OnDeactivate()
		{
			Debug.Assert(!IsDismissed);
			OnDeactivate();
		}

		/// <inheritdoc/>
		void IViewControllerEvents.OnPresent()
		{
			Debug.Assert(!IsDismissed);
			OnPresent();
		}

		/// <inheritdoc/>
		void IViewControllerEvents.OnDismiss()
		{
			OnDismiss();
		}

		/// <inheritdoc/>
		void IViewControllerEvents.OnUpdate(float frameTime)
		{
			Debug.Assert(!IsDismissed);
			OnUpdate(frameTime);
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
			ThrowIfDismissed();

			if (commandName is null)
			{
				throw new ArgumentNullException(nameof(commandName));
			}

			return OnCommand(commandName, args);
		}

		#endregion

		#region implementation

		private void OnCommand(object sender, CommandEventArgs e)
		{
			OnCommand(e.CommandName, e.CommandArguments);
		}

		#endregion
	}
}
