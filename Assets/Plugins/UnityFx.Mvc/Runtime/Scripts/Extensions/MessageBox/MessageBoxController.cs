// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Mvc.Extensions
{
	/// <summary>
	/// Controller of a generic message box.
	/// </summary>
	/// <seealso cref="MessageBoxView"/>
	/// <seealso cref="MessageBoxArgs"/>
	/// <seealso cref="MessageBoxOptions"/>
	/// <seealso cref="MessageBoxResult"/>
	[ViewController(PresentOptions = PresentOptions.Popup | PresentOptions.Modal)]
	public class MessageBoxController : IViewController, IViewControllerResult<MessageBoxResult>
	{
		#region data

		private readonly IPresentContext<MessageBoxResult> _context;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="MessageBoxController"/> class.
		/// </summary>
		public MessageBoxController(IPresentContext<MessageBoxResult> context)
		{
			_context = context;

			if (context.PresentArgs is MessageBoxArgs args && context.View is IConfigurable<MessageBoxArgs> view)
			{
				view.Configure(args);
			}
		}

		#endregion

		#region IViewController

		/// <inheritdoc/>
		public IView View => _context.View;

		#endregion

		#region ICommandTarget

		/// <inheritdoc/>
		public bool InvokeCommand(string commandName, object args)
		{
			if (_context.IsDismissed || commandName is null)
			{
				return false;
			}

			if (string.CompareOrdinal(commandName, ViewController.Commands.Ok) == 0)
			{
				_context.Dismiss(MessageBoxResult.Ok);
				return true;
			}

			if (string.CompareOrdinal(commandName, ViewController.Commands.Cancel) == 0 ||
				string.CompareOrdinal(commandName, ViewController.Commands.Close) == 0)
			{
				_context.Dismiss(MessageBoxResult.Cancel);
				return true;
			}

			return false;
		}

		#endregion

		#region implementation
		#endregion
	}
}
