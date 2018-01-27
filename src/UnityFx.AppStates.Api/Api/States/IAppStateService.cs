// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic application state manager.
	/// </summary>
	/// <seealso cref="IAppState"/>
	public interface IAppStateService : IAppStateManager, IDisposable
	{
		/// <summary>
		/// Triggered when a new state is pushed onto the state stack of this manager or any of its child managers.
		/// </summary>
		event EventHandler<AppStateEventArgs> StatePushed;

		/// <summary>
		/// Triggered when a state is popped from the state stack of this manager or any of its child managers.
		/// </summary>
		event EventHandler<AppStateEventArgs> StatePopped;

		/// <summary>
		/// Triggered when a state is activated (in this manager or in any of its child managers).
		/// </summary>
		event EventHandler<AppStateEventArgs> StateActivated;

		/// <summary>
		/// Triggered when a state is deactivated (in this manager or in any of its child managers).
		/// </summary>
		event EventHandler<AppStateEventArgs> StateDeactivated;

		/// <summary>
		/// Returns the service settings. Read only.
		/// </summary>
		IAppStateServiceSettings Settings { get; }
	}
}
