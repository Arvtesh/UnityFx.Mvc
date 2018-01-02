// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Returns a read-only stack of the <see cref="IAppState"/> instances.
	/// </summary>
	public interface IAppStateStack : IReadOnlyCollection<IAppState>
	{
		/// <summary>
		/// Triggered when a new state is added to the stack.
		/// </summary>
		event EventHandler<AppStateEventArgs> StateAdded;

		/// <summary>
		/// Triggered when a state is removed from the stack.
		/// </summary>
		event EventHandler<AppStateEventArgs> StateRemoved;

		/// <summary>
		/// Returns the state at the top of the stack without removing it.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the stack is empty.</exception>
		IAppState Peek();

		/// <summary>
		/// Returns the state at the top of the stack without removing it. Returns <see langword="false"/> if the stack is empty.
		/// </summary>
		bool TryPeek(out IAppState result);
	}
}
