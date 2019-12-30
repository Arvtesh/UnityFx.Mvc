// Copyright (c) 2018-2020 Alexander Bogarsukov.
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
		/// Enumerates controller-specific commands.
		/// </summary>
		public enum Commands
		{
			Ok,
			Cancel,
			Close
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MessageBoxController"/> class.
		/// </summary>
		public MessageBoxController(IPresentContext<MessageBoxResult> context)
		{
			_context = context;
			_context.View.Command += OnCommand;

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
		public bool InvokeCommand<TCommand>(TCommand command)
		{
			if (command != null && !_context.IsDismissed)
			{
				if (CommandWrapper<Commands>.TryUnpack(command, out var cmd))
				{
					if (cmd == Commands.Ok)
					{
						_context.Dismiss(MessageBoxResult.Ok);
					}
					else
					{
						_context.Dismiss(MessageBoxResult.Cancel);
					}

					return true;
				}
			}

			return false;
		}

		#endregion

		#region implementation

		private void OnCommand(object sender, CommandEventArgs e)
		{
			InvokeCommand(e);
		}

		#endregion
	}
}
