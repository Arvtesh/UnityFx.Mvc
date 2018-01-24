// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Extensions for <see cref="IAppStateManager"/>.
	/// </summary>
	public static class AppStateManagerExtensions
	{
		/// <summary>
		/// Pushes a new state on top of the current.
		/// </summary>
		public static Task<IAppState> PushStateAsync<TStateController>(this IAppStateManager stateManager, object args) where TStateController : class, IAppStateController
		{
			return stateManager.PushStateTaskAsync<TStateController>(PushOptions.None, args);
		}

		/// <summary>
		/// Pushes a new state on top of the current.
		/// </summary>
		public static Task<IAppState> PushStateAsync<TStateController>(this IAppStateManager stateManager) where TStateController : class, IAppStateController
		{
			return stateManager.PushStateTaskAsync<TStateController>(PushOptions.None, null);
		}

		/// <summary>
		/// Pushes a new state instead of the current.
		/// </summary>
		public static Task<IAppState> SetStateAsync<TStateController>(this IAppStateManager stateManager, object args) where TStateController : class, IAppStateController
		{
			return stateManager.PushStateTaskAsync<TStateController>(PushOptions.Set, args);
		}

		/// <summary>
		/// Pushes a new state instead of the current.
		/// </summary>
		public static Task<IAppState> SetStateAsync<TStateController>(this IAppStateManager stateManager) where TStateController : class, IAppStateController
		{
			return stateManager.PushStateTaskAsync<TStateController>(PushOptions.Set, null);
		}
	}
}
