// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
#if UNITYFX_SUPPORT_TAP
using System.Threading.Tasks;
#endif

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
	/// Enumerates <see cref="IAppStateController"/> push options.
	/// </summary>
	public enum PushOptions
	{
		/// <summary>
		/// No options.
		/// </summary>
		/// <seealso cref="AppStateOperationType.Push"/>
		None,

		/// <summary>
		/// Pushes new state onto the stack instead of the previous one.
		/// </summary>
		/// <seealso cref="AppStateOperationType.Set"/>
		Set,

		/// <summary>
		/// Pushes new state onto the stack instead of all other states.
		/// </summary>
		/// <seealso cref="AppStateOperationType.Reset"/>
		Reset
	}

	/// <summary>
	/// A generic application state manager.
	/// </summary>
	/// <threadsafety static="true" instance="false"/>
	/// <seealso href="http://gameprogrammingpatterns.com/state.html"/>
	/// <seealso cref="IAppState"/>
	/// <seealso cref="IAppStateController"/>
	public interface IAppStateManager : IAppStateContainer
	{
		/// <summary>
		/// Raised when a new push operation is initiated.
		/// </summary>
		/// <seealso cref="PushStateAsync(PushOptions, Type, object)"/>
		event EventHandler<PushStateInitiatedEventArgs> PushStateInitiated;

		/// <summary>
		/// Raised when a push operation is completed (either successfully or not).
		/// </summary>
		/// <seealso cref="PushStateAsync(PushOptions, Type, object)"/>
		event EventHandler<PushStateCompletedEventArgs> PushStateCompleted;

		/// <summary>
		/// Raised when a new pop operation is initiated.
		/// </summary>
		/// <seealso cref="PopStateAsync(IAppState)"/>
		event EventHandler<PopStateInitiatedEventArgs> PopStateInitiated;

		/// <summary>
		/// Raised when a pop operation is completed (either successfully or not).
		/// </summary>
		/// <seealso cref="PopStateAsync(IAppState)"/>
		event EventHandler<PopStateCompletedEventArgs> PopStateCompleted;

		/// <summary>
		/// Pushes a <paramref name="controllerType"/> instance on top of the states stack.
		/// </summary>
		/// <remarks>
		/// The method schedules an operation to run on the main thread. The operation instantiates a new state,
		/// the controller the specified type, loads it content and activates it.
		/// </remarks>
		/// <param name="options">Push options.</param>
		/// <param name="controllerType">Type of the state controller.</param>
		/// <param name="controllerArgs">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controllerType"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="controllerType"/> cannot be used to instantiate state controller (for instance it is abstract type).</exception>
		/// <exception cref="InvalidOperationException">Too many operations are scheduled already.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the manager is disposed.</exception>
		/// <seealso cref="PopStateAsync(IAppState)"/>
		IAppStateOperation<IAppState> PushStateAsync(PushOptions options, Type controllerType, object controllerArgs);

		/// <summary>
		/// Removes the specified state from the stack.
		/// </summary>
		/// <param name="state">The state to remove.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="state"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the <paramref name="state"/> does not belong to this manager.</exception>
		/// <seealso cref="PushStateAsync(PushOptions, Type, object)"/>
		IAppStateOperation PopStateAsync(IAppState state);

#if UNITYFX_SUPPORT_TAP

		/// <summary>
		/// Pushes a <paramref name="controllerType"/> instance on top of the states stack.
		/// </summary>
		/// <remarks>
		/// The method schedules an operation to run on the main thread. The operation instantiates a new state,
		/// the controller the specified type, loads it content and activates it.
		/// </remarks>
		/// <param name="options">Push options.</param>
		/// <param name="controllerType">Type of the state controller.</param>
		/// <param name="controllerArgs">Controller arguments.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controllerType"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="controllerType"/> cannot be used to instantiate state controller (for instance it is abstract type).</exception>
		/// <exception cref="InvalidOperationException">Too many operations are scheduled already.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the manager is disposed.</exception>
		/// <seealso cref="PopStateTaskAsync(IAppState)"/>
		Task<IAppState> PushStateTaskAsync(PushOptions options, Type controllerType, object controllerArgs);

		/// <summary>
		/// Removes the specified state from the stack.
		/// </summary>
		/// <param name="state">The state to remove.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="state"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the <paramref name="state"/> does not belong to this manager.</exception>
		/// <seealso cref="PushStateTaskAsync(PushOptions, Type, object)"/>
		Task PopStateTaskAsync(IAppState state);

#endif
	}
}
