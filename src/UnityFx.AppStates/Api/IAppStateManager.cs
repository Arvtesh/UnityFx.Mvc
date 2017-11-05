// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnityFx.App
{
	/// <summary>
	/// A generic application state controller.
	/// </summary>
	/// <seealso cref="IAppState"/>
	public interface IAppStateManager
	{
		/// <summary>
		/// Returns the child states stack. Read only.
		/// </summary>
		IAppStateStack States { get; }

		/// <summary>
		/// Enumerates child states recursively. Read only.
		/// </summary>
		IEnumerable<IAppState> StatesRecursive { get; }

		/// <summary>
		/// Removes all states from the stack and pushes <typeparamref name="T"/> instance.
		/// </summary>
		/// <param name="args">State-specific arguments.</param>
		/// <typeparam name="T">The state type.</typeparam>
		void SetState<T>(object args = null) where T : class, IAppStateController;

		/// <summary>
		/// Removes all states from the stack and pushes <typeparamref name="T"/> instance.
		/// </summary>
		/// <param name="args">State-specific arguments.</param>
		/// <typeparam name="T">The state type.</typeparam>
		Task<T> SetStateAsync<T>(object args = null) where T : class, IAppStateController;

		/// <summary>
		/// Removes all states from the stack and pushes instance of <paramref name="stateType"/> type.
		/// </summary>
		/// <param name="stateType">Type of the new state.</param>
		/// <param name="args">State arguments.</param>
		void SetState(Type stateType, object args = null);

		/// <summary>
		/// Removes all states from the stack and pushes instance of <paramref name="stateType"/> type.
		/// </summary>
		/// <param name="stateType">Type of the new state.</param>
		/// <param name="args">State arguments.</param>
		Task<IAppState> SetStateAsync(Type stateType, object args = null);
	}
}
