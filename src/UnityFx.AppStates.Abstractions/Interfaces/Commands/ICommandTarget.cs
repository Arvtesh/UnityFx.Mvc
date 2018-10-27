// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Represents an object capable of command processing.
	/// </summary>
	/// <seealso cref="INotifyCommand"/>
	public interface ICommandTarget
	{
		/// <summary>
		/// Invokes a command. An implementation might choose to ignore the command, in this case the method should return <see langword="false"/>.
		/// </summary>
		/// <param name="commandName">Name of the command to invoke.</param>
		/// <param name="args">Command-specific arguments.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="commandName"/> is <see langword="null"/>.</exception>
		/// <returns>Returns <see langword="true"/> if the command has been handles; <see langword="false"/> otherwise.</returns>
		bool InvokeCommand(string commandName, object args);
	}
}
