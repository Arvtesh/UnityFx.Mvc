// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Event arguments for an arbitraty action.
	/// </summary>
	/// <typeparam name="T">Type of the command arguments.</typeparam>
	public class CommandEventArgs<TCommand, TArgs> : CommandEventArgs<TCommand>
	{
		/// <summary>
		/// Gets the command arguments.
		/// </summary>
		public TArgs Args { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CommandEventArgs{TArgs}"/> class.
		/// </summary>
		public CommandEventArgs(TCommand command, TArgs args)
			: base(command)
		{
			Args = args;
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			if (Args != null)
			{
				return base.ToString() + '(' + Args.ToString() + ')';
			}

			return base.ToString();
		}
	}
}
