// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Event arguments for an arbitraty action.
	/// </summary>
	public class CommandEventArgs : EventArgs
	{
		#region data

		private readonly string _commandName;
		private readonly object _commandArgs;

		#endregion

		#region interface

		/// <summary>
		/// Gets the command name.
		/// </summary>
		public string CommandName => _commandName;

		/// <summary>
		/// Gets the command arguments.
		/// </summary>
		public object CommandArguments => _commandArgs;

		/// <summary>
		/// Initializes a new instance of the <see cref="CommandEventArgs"/> class.
		/// </summary>
		public CommandEventArgs(string commandName)
		{
			_commandName = commandName;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CommandEventArgs"/> class.
		/// </summary>
		public CommandEventArgs(string commandName, object args)
		{
			_commandName = commandName;
			_commandArgs = args;
		}

		#endregion
	}
}
