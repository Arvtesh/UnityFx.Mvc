// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Event arguments for an arbitraty action.
	/// </summary>
	public interface ICommandResultAccess<TArgs>
	{
		/// <summary>
		/// Gets the command arguments.
		/// </summary>
		TArgs Args { get; }
	}
}
