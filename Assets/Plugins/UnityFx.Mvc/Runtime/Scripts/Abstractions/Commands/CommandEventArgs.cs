// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Event arguments for an arbitraty action.
	/// </summary>
	public class CommandEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the command name.
		/// </summary>
		public string CommandName { get; }

		/// <summary>
		/// Gets the command arguments.
		/// </summary>
		public object CommandArguments { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CommandEventArgs"/> class.
		/// </summary>
		public CommandEventArgs(string commandName)
		{
			CommandName = commandName;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CommandEventArgs"/> class.
		/// </summary>
		public CommandEventArgs(string commandName, object args)
		{
			CommandName = commandName;
			CommandArguments = args;
		}
	}
}
