// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Mvc.Extensions
{
	/// <summary>
	/// Controller of a generic dialog.
	/// </summary>
	/// <remarks>
	/// Can work with any view that implements <see cref="IConfigurable{T}"/>.
	/// </remarks>
	/// <seealso cref="DialogArgs"/>
	public abstract class DialogController<TResult, TArgs> : IViewController, IViewControllerResult<TResult>, IViewControllerArgs<TArgs>, ICommandTarget where TArgs : DialogArgs
	{
		#region data

		private readonly IPresentContext<TResult> _context;

		#endregion

		#region interface

		/// <summary>
		/// Gets the controller context.
		/// </summary>
		protected IPresentContext<TResult> Context => _context;

		/// <summary>
		/// Gets a value indicating whether the dialog is dismissed.
		/// </summary>
		/// <seealso cref="Dismiss(TResult)"/>
		protected bool IsDismissed => _context.IsDismissed;

		/// <summary>
		/// Initializes a new instance of the <see cref="DialogController"/> class.
		/// </summary>
		protected DialogController(IPresentContext<TResult> context)
		{
			_context = context;
		}

		/// <summary>
		/// Dismissses this controller.
		/// </summary>
		/// <seealso cref="IsDismissed"/>
		protected void Dismiss(TResult result)
		{
			_context.Dismiss(result);
		}

		/// <summary>
		/// Called to process a command.
		/// </summary>
		/// <returns>Returns <see langword="true"/> if the command has been handles; <see langword="false"/> otherwise.</returns>
		protected virtual bool OnCommand(Command command, Variant args)
		{
			return false;
		}

		#endregion

		#region IViewController

		/// <inheritdoc/>
		public IView View => _context.View;

		#endregion

		#region ICommandTarget

		/// <inheritdoc/>
		public bool InvokeCommand(Command command, Variant args)
		{
			if (!command.IsNull && !_context.IsDismissed)
			{
				return OnCommand(command, args);
			}

			return false;
		}

		#endregion

		#region implementation

		private void OnCommand(object sender, CommandEventArgs e)
		{
			if (e != null && !_context.IsDismissed)
			{
				OnCommand(e.Command, e.Args);
			}
		}

		#endregion
	}
}
