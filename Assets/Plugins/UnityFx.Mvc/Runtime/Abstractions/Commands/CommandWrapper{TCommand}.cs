// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Wrapper of a generic command.
	/// </summary>
	/// <typeparam name="TCommand">Type of the command.</typeparam>
	public readonly struct CommandWrapper<TCommand>
	{
		private readonly TCommand _command;
		private readonly CommandEventArgs<TCommand> _args;

		/// <summary>
		/// Gets the command.
		/// </summary>
		public TCommand Command => _command;

		/// <summary>
		/// Initializes a new instance of the <see cref="CommandWrapper"/> struct.
		/// </summary>
		private CommandWrapper(TCommand command, CommandEventArgs<TCommand> args)
		{
			_command = command;
			_args = args;
		}

		/// <summary>
		/// Returns command arguments of the specified type (if any).
		/// </summary>
		/// <typeparam name="T">Type of the argyments.</typeparam>
		public T GetArgs<T>()
		{
			if (_args is CommandEventArgs<TCommand, T> ea)
			{
				return ea.Args;
			}

			return default;
		}

		/// <summary>
		/// Attepts to create an instance of <see cref="CommandWrapper{TCommand}"/>.
		/// </summary>
		/// <typeparam name="TPackedCommand">Type of the packed command.</typeparam>
		/// <param name="command">Packed command.</param>
		/// <param name="commandWrapper">The operation result.</param>
		/// <returns>Returns <see langword="true"/> if <paramref name="command"/> matches the <typeparamref name="TCommand"/>; <see langword="false"/> otherwise.</returns>
		public static bool TryUnpack<TPackedCommand>(TPackedCommand command, out CommandWrapper<TCommand> commandWrapper)
		{
			if (command is CommandEventArgs<TCommand> ea)
			{
				commandWrapper = new CommandWrapper<TCommand>(ea.Command, ea);
				return true;
			}
			
			if (command is TCommand c)
			{
				commandWrapper = new CommandWrapper<TCommand>(c, null);
				return true;
			}

			commandWrapper = default;
			return false;
		}

		/// <summary>
		/// Implicit conversion to <typeparamref name="TCommand"/>.
		/// </summary>
		public static implicit operator TCommand(CommandWrapper<TCommand> wrapper)
		{
			return wrapper._command;
		}
	}
}
