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
		/// Returns app states. Read only.
		/// </summary>
		AppStateStack StatesEx { get; }

		/// <summary>
		/// Returns the parent state. Read only.
		/// </summary>
		AppState ParentState { get; }

		/// <summary>
		/// Creates a new instance of <see cref="IAppStateManager"/> for the specified state's children.
		/// </summary>
		IAppStateManagerInternal CreateSubstateManager(AppState state);

		/// <summary>
		/// Create s a new view for the specified state.
		/// </summary>
		IAppView CreateView(AppState state);

		/// <summary>
		/// Pushes a new state onto the state stack.
		/// </summary>
		Task<IAppState> PushState(AppState ownerState, Type controllerType, PushOptions options, object stateArgs);

		/// <summary>
		/// Pops an existing state from the state stack.
		/// </summary>
		Task PopState(AppState state);

		/// <summary>
		/// Clears the state stack.
		/// </summary>
		Task PopAll();

		/// <summary>
		/// 
		/// </summary>
		void ActivateTopState();

		/// <summary>
		/// 
		/// </summary>
		void DeactivateTopState();
	}
}
