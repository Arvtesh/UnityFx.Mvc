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
	public interface IAppStateService : IDisposable
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

		/// <summary>
		/// Pushes a <paramref name="controllerType"/> instance on top of the states stack.
		/// </summary>
		/// <remarks>
		/// The method schedules an operation to run on the main thread. The operation instantiates a new state,
		/// a controller of the specified type, loads it content and activates it.
		/// </remarks>
		/// <param name="controllerType">Type of the state controller.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controllerType"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="controllerType"/> cannot be used to instantiate state controller (for instance it is abstract type).</exception>
		/// <exception cref="InvalidOperationException">Too many operations are scheduled already.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the manager is disposed.</exception>
		/// <seealso href="https://docs.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/event-based-asynchronous-pattern-eap">Event-based Asynchronous Pattern (EAP)</seealso>
		IAsyncOperation<AppViewController> PresentAsync(Type controllerType, PresentArgs args);
	}
}
