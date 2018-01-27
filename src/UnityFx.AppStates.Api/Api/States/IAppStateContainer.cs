// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A container for <see cref="IAppState"/> instance.
	/// </summary>
	public interface IAppStateContainer
	{
		/// <summary>
		/// Returns the child states stack. Read only.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown if the manager is disposed.</exception>
		IAppStateStack States { get; }

		/// <summary>
		/// Enumerates child states recursively.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown if the manager is disposed.</exception>
		IEnumerable<IAppState> GetStatesRecursive();

		/// <summary>
		/// Enumerates child states recursively.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown if the manager is disposed.</exception>
		void GetStatesRecursive(ICollection<IAppState> states);
	}
}
