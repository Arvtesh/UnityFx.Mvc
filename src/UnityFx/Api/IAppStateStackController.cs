// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace UnityFx.App
{
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
		/// <param name="args">State-specific arguments.</param>
		/// <typeparam name="T">The state type.</typeparam>
		void PushState<T>(object args = null) where T : class, IAppState;

		/// <summary>
		/// Pushes a <typeparamref name="T"/> instance on top of the caller <see cref="IAppState"/>.
		/// </summary>
		/// <param name="args">State-specific arguments.</param>
		/// <typeparam name="T">The state type.</typeparam>
		Task<T> PushStateAsync<T>(object args = null) where T : class, IAppState;

		/// <summary>
		/// Pushes a <paramref name="stateType"/> instance on top of the caller <see cref="IAppState"/>.
		/// </summary>
		/// <param name="stateType">Type of the new state.</param>
		/// <param name="args">State arguments.</param>
		void PushState(Type stateType, object args = null);

		/// <summary>
		/// Pushes a <paramref name="stateType"/> instance on top of the caller <see cref="IAppState"/>.
		/// </summary>
		/// <param name="stateType">Type of the new state.</param>
		/// <param name="args">State arguments.</param>
		Task<IAppState> PushStateAsync(Type stateType, object args = null);

		/// <summary>
		/// Replaces the caller <see cref="IAppState"/> (and all child states) with <typeparamref name="T"/> instance.
		/// </summary>
		/// <param name="args">State-specific arguments.</param>
		/// <typeparam name="T">The state type.</typeparam>
		void SetState<T>(object args = null) where T : class, IAppState;

		/// <summary>
		/// Replaces the caller <see cref="IAppState"/> (and all child states) with <typeparamref name="T"/> instance.
		/// </summary>
		/// <param name="args">State-specific arguments.</param>
		/// <typeparam name="T">The state type.</typeparam>
		Task<T> SetStateAsync<T>(object args = null) where T : class, IAppState;

		/// <summary>
		/// Replaces the caller <see cref="IAppState"/> (and all child states) with instance of <paramref name="stateType"/> type.
		/// </summary>
		/// <param name="stateType">Type of the new state.</param>
		/// <param name="args">State arguments.</param>
		void SetState(Type stateType, object args = null);

		/// <summary>
		/// Replaces the caller <see cref="IAppState"/> (and all child states) with instance of <paramref name="stateType"/> type.
		/// </summary>
		/// <param name="stateType">Type of the new state.</param>
		/// <param name="args">State arguments.</param>
		Task<IAppState> SetStateAsync(Type stateType, object args = null);

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
