// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.App
{
	/// <summary>
	/// Represents <see cref="IAppState"/> context.
	/// </summary>
	/// <seealso cref="IAppState"/>
	/// <seealso cref="IAppStateEvents"/>
	public interface IAppStateContext : IAppStateInfo, IAppStateStackController
	{
		/// <summary>
		/// Returns the user-defined application context data. Read only.
		/// </summary>
		object AppContext { get; }

		/// <summary>
		/// Returns the substate manager for this state. Never returns <c>null</c>. Read only.
		/// </summary>
		IAppStateManager SubstateManager { get; }
	}
}
