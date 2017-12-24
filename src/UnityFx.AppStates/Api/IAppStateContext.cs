// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.App
{
	/// <summary>
	/// Represents <see cref="IAppStateController"/> context.
	/// </summary>
	/// <seealso cref="IAppState"/>
	public interface IAppStateContext
	{
		/// <summary>
		/// Returns user-specified controller arguments. Read only.
		/// </summary>
		object Args { get; }

		/// <summary>
		/// Returns parent state. Never returns <c>null</c>. Read only.
		/// </summary>
		IAppState State { get; }

		/// <summary>
		/// Returns a view attached to the state. Never returns <c>null</c>. Read only.
		/// </summary>
		IAppView View { get; }

		/// <summary>
		/// Returns the parent state manager. Never returns <c>null</c>. Read only.
		/// </summary>
		IAppStateManager StateManager { get; }

		/// <summary>
		/// Returns the substate manager for this state. Never returns <c>null</c>. Read only.
		/// </summary>
		IAppStateManager SubstateManager { get; }
	}
}
