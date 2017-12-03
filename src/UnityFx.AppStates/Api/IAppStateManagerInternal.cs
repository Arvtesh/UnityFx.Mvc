// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnityFx.App
{
	/// <summary>
	/// A state manager interface consumed by <see cref="AppState"/>.
	/// </summary>
	internal interface IAppStateManagerInternal : IAppStateManager
	{
		/// <summary>
		/// Returns the application context. Read only.
		/// </summary>
		object AppContext { get; }

		/// <summary>
		/// Returns the parent state. Read only.
		/// </summary>
		IAppStateInternal ParentState { get; }

		/// <summary>
		/// Creates a new instance of <see cref="IAppStateManager"/> for the specified state's children.
		/// </summary>
		IAppStateManagerInternal CreateSubstateManager(IAppStateInternal state);

		/// <summary>
		/// Create s a new view for the specified state.
		/// </summary>
		IAppView CreateView(IAppStateInternal state);

		/// <summary>
		/// Pushes a new state on the state stack.
		/// </summary>
		void PushState(IAppStateInternal ownerState, Type controllerType, PushOptions options, object stateArgs);

		/// <summary>
		/// Pushes a new state on the state stack.
		/// </summary>
		Task<IAppState> PushStateAsync(IAppStateInternal ownerState, Type controllerType, PushOptions options, object stateArgs);

		/// <summary>
		/// Pops an existing state from the state stack.
		/// </summary>
		void PopState(IAppStateInternal state);

		/// <summary>
		/// Pops an existing state from the state stack.
		/// </summary>
		Task PopStateAsync(IAppStateInternal state);

		/// <summary>
		/// Clears the state stack.
		/// </summary>
		void PopAll();
	}
}
