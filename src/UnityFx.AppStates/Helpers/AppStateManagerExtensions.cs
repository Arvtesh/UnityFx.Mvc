// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace UnityFx.App
{
	/// <summary>
	/// Extensions for <see cref="IAppStateManager"/>.
	/// </summary>
	public static class AppStateManagerExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		public static Task<IAppState> PushStateAsync<TStateController>(this IAppStateManager stateManager, object args) where TStateController : class, IAppStateController
		{
			return stateManager.PushStateAsync<TStateController>(PushOptions.None, args);
		}

		/// <summary>
		/// 
		/// </summary>
		public static Task<IAppState> PushStateAsync<TStateController>(this IAppStateManager stateManager) where TStateController : class, IAppStateController
		{
			return stateManager.PushStateAsync<TStateController>(PushOptions.None, null);
		}

		/// <summary>
		/// 
		/// </summary>
		public static Task<IAppState> SetStateAsync<TStateController>(this IAppStateManager stateManager, object args) where TStateController : class, IAppStateController
		{
			return stateManager.PushStateAsync<TStateController>(PushOptions.Set, args);
		}

		/// <summary>
		/// 
		/// </summary>
		public static Task<IAppState> SetStateAsync<TStateController>(this IAppStateManager stateManager) where TStateController : class, IAppStateController
		{
			return stateManager.PushStateAsync<TStateController>(PushOptions.Set, null);
		}
	}
}
