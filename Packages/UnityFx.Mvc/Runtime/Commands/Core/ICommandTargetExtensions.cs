// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Extensions of <see cref="ICommandTarget"/>.
	/// </summary>
	/// <seealso cref="ICommandTarget"/>
	public static class ICommandTargetExtensions
	{
		/// <summary>
		/// Invokes a command. An implementation might choose to ignore the command, in this case the method should return <see langword="false"/>.
		/// </summary>
		/// <param name="command">Command to invoke.</param>
		/// <param name="args">Command arguments.</param>
		/// <returns>Returns <see langword="true"/> if the command has been handled; <see langword="false"/> otherwise.</returns>
		public static bool InvokeCommand<TCommand>(this ICommandTarget commandTarget, TCommand command)
		{
			return commandTarget.InvokeCommand(Command.FromType(command), new Variant());
		}

		/// <summary>
		/// Invokes a command. An implementation might choose to ignore the command, in this case the method should return <see langword="false"/>.
		/// </summary>
		/// <param name="command">Command to invoke.</param>
		/// <param name="args">Command arguments.</param>
		/// <returns>Returns <see langword="true"/> if the command has been handled; <see langword="false"/> otherwise.</returns>
		public static bool InvokeCommand<TCommand>(this ICommandTarget commandTarget, TCommand command, Variant args)
		{
			return commandTarget.InvokeCommand(Command.FromType(command), args);
		}
	}
}
