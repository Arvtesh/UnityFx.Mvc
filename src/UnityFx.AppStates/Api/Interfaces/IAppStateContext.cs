// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Represents state controller context.
	/// </summary>
	/// <seealso cref="IAppState"/>
	public interface IAppStateContext
	{
		/// <summary>
		/// Returns user-specified controller arguments.
		/// </summary>
		object Args { get; }

		/// <summary>
		/// Returns parent state. Never returns <see langword="null"/>.
		/// </summary>
		IAppState State { get; }

		/// <summary>
		/// Returns a view attached to the state. Never returns <see langword="null"/>.
		/// </summary>
		IAppStateView View { get; }

		/// <summary>
		/// Returns the parent state manager. Never returns <see langword="null"/>.
		/// </summary>
		IAppStateManager StateManager { get; }

		/// <summary>
		/// Returns the substate manager for this state. Never returns <see langword="null"/>.
		/// </summary>
		IAppStateManager SubstateManager { get; }
	}
}
