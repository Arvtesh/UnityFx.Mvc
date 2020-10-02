﻿// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Represents an object capable of command processing.
	/// </summary>
	/// <seealso cref="INotifyCommand"/>
	public interface ICommandTarget<T>
	{
		/// <summary>
		/// Invokes a command. An implementation might choose to ignore the command, in this case the method should return <see langword="false"/>.
		/// </summary>
		/// <param name="command">Command to invoke.</param>
		/// <param name="args">The command arguments.</param>
		/// <returns>Returns <see langword="true"/> if the command has been handled; <see langword="false"/> otherwise.</returns>
		bool InvokeCommand(T command, Variant args);
	}
}
