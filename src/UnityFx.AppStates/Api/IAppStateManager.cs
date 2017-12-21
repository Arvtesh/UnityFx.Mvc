// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
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
	/// A generic application state manager.
	/// </summary>
	/// <seealso href="http://gameprogrammingpatterns.com/state.html"/>
	/// <seealso cref="IAppState"/>
	public interface IAppStateManager
	{
		/// <summary>
		/// Returns the child states stack. Read only.
		/// </summary>
		IAppStateStack States { get; }

		/// <summary>
		/// Enumerates child states recursively.
		/// </summary>
		IEnumerable<IAppState> GetStatesRecursive();

		/// <summary>
		/// Enumerates child states recursively.
		/// </summary>
		void GetStatesRecursive(ICollection<IAppState> states);

		/// <summary>
		/// Pushes a <typeparamref name="TStateController"/> instance on top of the states stack.
		/// </summary>
		/// <param name="options">Push options.</param>
		/// <param name="args">Controller-specific arguments.</param>
		/// <typeparam name="TStateController">Type of the state controller.</typeparam>
		Task<IAppState> PushStateAsync<TStateController>(PushOptions options, object args) where TStateController : class, IAppStateController;

		/// <summary>
		/// Pushes a <paramref name="controllerType"/> instance on top of the states stack.
		/// </summary>
		/// <param name="controllerType">Type of the state controller.</param>
		/// <param name="options">Push options.</param>
		/// <param name="args">Controller arguments.</param>
		Task<IAppState> PushStateAsync(Type controllerType, PushOptions options, object args);
	}
}
