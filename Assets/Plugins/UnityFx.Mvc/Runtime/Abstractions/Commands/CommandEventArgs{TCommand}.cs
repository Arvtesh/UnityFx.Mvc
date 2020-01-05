// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Event arguments for an arbitraty action.
	/// </summary>
	public class CommandEventArgs<TCommand> : CommandEventArgs, ICommandAccess<TCommand>
	{
		/// <summary>
		/// Gets the command name.
		/// </summary>
		public TCommand Command { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CommandEventArgs"/> class.
		/// </summary>
		public CommandEventArgs(TCommand command)
		{
			Command = command;
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			if (Command != null)
			{
				return Command.ToString();
			}

			return "Null";
		}
	}
}
