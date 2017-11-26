// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnityFx.App
{
	/// <summary>
	/// A state manager interface .
	/// </summary>
	/// <seealso cref="IAppState"/>
	internal interface IAppStateManagerInternal : IAppStateManager
	{
		/// <summary>
		/// Returns the application context. Read only.
		/// </summary>
		object AppContext { get; }

		/// <summary>
		/// Returns the parent state. Read only.
		/// </summary>
		IAppState ParentState { get; }

		/// <summary>
		/// Creates a new instance of <see cref="IAppStateManager"/> for the specified state's children.
		/// </summary>
		IAppStateManager CreateSubstateManager(IAppState state);

		/// <summary>
		/// Create s a new view for the specified state.
		/// </summary>
		IAppView CreateView(IAppState state);

		/// <summary>
		/// Pushes a new state on the state stack.
		/// </summary>
		void PushState(IAppState ownerState, Type controllerType, PushOptions options, object stateArgs);

		/// <summary>
		/// Pushes a new state on the state stack.
		/// </summary>
		Task<IAppState> PushStateAsync(IAppState ownerState, Type controllerType, PushOptions options, object stateArgs);

		/// <summary>
		/// Pops an existing state from the state stack.
		/// </summary>
		void PopState(IAppState state);

		/// <summary>
		/// Pops an existing state from the state stack.
		/// </summary>
		Task PopStateAsync(IAppState state);

		/// <summary>
		/// Instantly removes the specified state from the stack. Called when disposing a state.
		/// </summary>
		void ReleaseState(IAppState state);
	}
}
