// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
	public interface IAppStateManager : IEnumerable<IAppState>
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
		/// Triggered when a state operation is complete (in this manager or in any of its child managers).
		/// </summary>
		event EventHandler<AppStateOperationEventArgs> StateOperationCompleted;

		/// <summary>
		/// Returns the child states stack. Read only.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown if the manager is disposed.</exception>
		IAppStateStack States { get; }

		/// <summary>
		/// Enumerates child states recursively.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown if the manager is disposed.</exception>
		IEnumerable<IAppState> GetStatesRecursive();

		/// <summary>
		/// Enumerates child states recursively.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown if the manager is disposed.</exception>
		void GetStatesRecursive(ICollection<IAppState> states);

		/// <summary>
		/// Pushes a <typeparamref name="TStateController"/> instance on top of the states stack.
		/// </summary>
		/// <remarks>
		/// The method schedules an operation to run on the main thread. The operation instantiates a new state,
		/// the controller the specified type, loads it content and activates it.
		/// </remarks>
		/// <param name="options">Push options.</param>
		/// <param name="args">Controller-specific arguments.</param>
		/// <typeparam name="TStateController">Type of the state controller.</typeparam>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TStateController"/> cannot be used to instantiate state controller (for instance it is abstract type).</exception>
		/// <exception cref="InvalidOperationException">Too many operations are scheduled already.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the manager is disposed.</exception>
		/// <seealso cref="PushStateAsync(Type, PushOptions, object)"/>
		Task<IAppState> PushStateAsync<TStateController>(PushOptions options, object args) where TStateController : class, IAppStateController;

		/// <summary>
		/// Pushes a <paramref name="controllerType"/> instance on top of the states stack.
		/// </summary>
		/// <remarks>
		/// The method schedules an operation to run on the main thread. The operation instantiates a new state,
		/// the controller the specified type, loads it content and activates it.
		/// </remarks>
		/// <param name="controllerType">Type of the state controller.</param>
		/// <param name="options">Push options.</param>
		/// <param name="args">Controller arguments.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controllerType"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="controllerType"/> cannot be used to instantiate state controller (for instance it is abstract type).</exception>
		/// <exception cref="InvalidOperationException">Too many operations are scheduled already.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the manager is disposed.</exception>
		/// <seealso cref="PushStateAsync{TStateController}(PushOptions, object)"/>
		Task<IAppState> PushStateAsync(Type controllerType, PushOptions options, object args);
	}
}
