// Copyright (c) 2018-2020 Alexander Bogarsukov.
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
		/// The command.
		/// </summary>
		public readonly Command Command;

		/// <summary>
		/// The command arguments.
		/// </summary>
		public readonly Variant Args;

		/// <summary>
		/// Initializes a new instance of the <see cref="CommandEventArgs"/> class.
		/// </summary>
		public CommandEventArgs()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CommandEventArgs"/> class.
		/// </summary>
		public CommandEventArgs(Command command, Variant args)
		{
			Command = command;
			Args = args;
		}
	}
}
