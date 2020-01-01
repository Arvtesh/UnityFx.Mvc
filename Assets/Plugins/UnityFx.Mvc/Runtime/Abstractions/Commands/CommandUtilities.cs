// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Command-related helpers.
	/// </summary>
	public static class CommandUtilities
	{
		/// <summary>
		/// Attepts to unpack a command.
		/// </summary>
		public static bool TryUnpack<TPackedCommand, TCommand>(TPackedCommand packedCommand, out TCommand command)
		{
			if (packedCommand is ICommandAccess<TCommand> ea)
			{
				command = ea.Command;
				return true;
			}

			if (packedCommand is TCommand c)
			{
				command = c;
				return true;
			}

			command = default;
			return false;
		}

		/// <summary>
		/// Attempts to unpack command arguments.
		/// </summary>
		public static bool TryUnpackArgs<TPackedCommand, TArgs>(TPackedCommand packedCommand, out TArgs args)
		{
			if (packedCommand is ICommandResultAccess<TArgs> ea)
			{
				args = ea.Args;
				return true;
			}

			args = default;
			return false;
		}
	}
}
