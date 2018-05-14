// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Enumerated state operation types.
	/// </summary>
	public enum AppStateOperationType
	{
		/// <summary>
		/// Pushes a new state on top of the stack.
		/// </summary>
		Push,

		/// <summary>
		/// Pushes a new state instead of the caller.
		/// </summary>
		Set,

		/// <summary>
		/// Pushes a new state instead of all other states.
		/// </summary>
		Reset,

		/// <summary>
		/// Pops an existing state from the stack.
		/// </summary>
		Pop,

		/// <summary>
		/// Pops all states from the stack.
		/// </summary>
		PopAll
	}

	/// <summary>
	/// A generic application state service.
	/// </summary>
	/// <seealso cref="AppState"/>
	public interface IAppStateService : IPresenter, IDisposable
	{
		/// <summary>
		/// Raised when a new push operation is initiated.
		/// </summary>
		event EventHandler<PresentInitiatedEventArgs> PresentInitiated;

		/// <summary>
		/// Raised when a push operation is completed (either successfully or not).
		/// </summary>
		event EventHandler<PresentCompletedEventArgs> PresentCompleted;

		/// <summary>
		/// Raised when a new pop operation is initiated.
		/// </summary>
		event EventHandler<DismissInitiatedEventArgs> DismissInitiated;

		/// <summary>
		/// Raised when a pop operation is completed (either successfully or not).
		/// </summary>
		event EventHandler<DismissCompletedEventArgs> DismissCompleted;

		/// <summary>
		/// Gets the service settings.
		/// </summary>
		AppStateServiceSettings Settings { get; }

		/// <summary>
		/// Gets the child states.
		/// </summary>
		AppStateCollection States { get; }

		/// <summary>
		/// Gets a value indicating whether thare are any pending operations.
		/// </summary>
		bool IsBusy { get; }
	}
}
