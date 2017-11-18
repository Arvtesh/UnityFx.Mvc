// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace UnityFx.App
{
	/// <summary>
	/// Enumerates <see cref="IAppStateController"/> push options.
	/// </summary>
	public enum PushOptions
	{
		/// <summary>
		/// No options.
		/// </summary>
		None,

		/// <summary>
		/// Pushes new state onto the stack instead of the previous one.
		/// </summary>
		Set,

		/// <summary>
		/// Pushes new state onto the stack instead of all other states.
		/// </summary>
		Reset
	}

	/// <summary>
	/// Enumerates state operations.
	/// </summary>
	public enum StackOperation
	{
		/// <summary>
		/// Push a new state on top of the previous one.
		/// </summary>
		Push,

		/// <summary>
		/// Removes the state from the stack.
		/// </summary>
		Pop,
	}

	/// <summary>
	/// A generic state manager attached to a <see cref="IAppState"/> instance.
	/// </summary>
	/// <seealso href="http://gameprogrammingpatterns.com/state.html"/>
	/// <seealso cref="IAppState"/>
	public interface IAppStateStackController
	{
		/// <summary>
		/// Pushes a <typeparamref name="T"/> instance on top of the caller <see cref="IAppState"/>.
		/// </summary>
		/// <param name="options">Push options.</param>
		/// <param name="args">State-specific arguments.</param>
		/// <typeparam name="T">The state type.</typeparam>
		void PushState<T>(PushOptions options = PushOptions.None, object args = null) where T : class, IAppStateController;

		/// <summary>
		/// Pushes a <typeparamref name="T"/> instance on top of the caller <see cref="IAppState"/>.
		/// </summary>
		/// <param name="options">Push options.</param>
		/// <param name="args">State-specific arguments.</param>
		/// <typeparam name="T">The state type.</typeparam>
		Task<IAppState> PushStateAsync<T>(PushOptions options = PushOptions.None, object args = null) where T : class, IAppStateController;

		/// <summary>
		/// Pushes a <paramref name="controllerType"/> instance on top of the caller <see cref="IAppState"/>.
		/// </summary>
		/// <param name="options">Push options.</param>
		/// <param name="controllerType">Type of the new state.</param>
		/// <param name="args">State arguments.</param>
		void PushState(Type controllerType, PushOptions options = PushOptions.None, object args = null);

		/// <summary>
		/// Pushes a <paramref name="controllerType"/> instance on top of the caller <see cref="IAppState"/>.
		/// </summary>
		/// <param name="controllerType">Type of the new state.</param>
		/// <param name="options">Push options.</param>
		/// <param name="args">State arguments.</param>
		Task<IAppState> PushStateAsync(Type controllerType, PushOptions options = PushOptions.None, object args = null);

		/// <summary>
		/// Pops the caller <see cref="IAppState"/> (and all child states) from the state stack and activates previous one (if any).
		/// </summary>
		void PopState();

		/// <summary>
		/// Pops the caller <see cref="IAppState"/> (and all child states) from the state stack and activates previous one (if any).
		/// </summary>
		Task PopStateAsync();
	}
}
