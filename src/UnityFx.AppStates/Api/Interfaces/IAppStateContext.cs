// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Represents state controller context.
	/// </summary>
	/// <seealso cref="AppState"/>
	public interface IAppStateContext
	{
		/// <summary>
		/// Gets the arguments used to create the state.
		/// </summary>
		PushStateArgs CreationArgs { get; }

		/// <summary>
		/// Gets parent state.
		/// </summary>
		AppState State { get; }

		/// <summary>
		/// Gets a view attached to the state.
		/// </summary>
		IAppStateView View { get; }

		/// <summary>
		/// Gets the parent state manager.
		/// </summary>
		IAppStateManager StateManager { get; }

		/// <summary>
		/// Gets a substate manager for this state.
		/// </summary>
		IAppStateManager SubstateManager { get; }
	}
}
