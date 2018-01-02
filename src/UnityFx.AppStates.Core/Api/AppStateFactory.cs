// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;
using UnityEngine;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Assembly entry point.
	/// </summary>
	public static class AppStateFactory
	{
		/// <summary>
		/// Returns <see cref="Type"/> for the <see cref="IAppStateService"/> implementated in the assembly.
		/// Intended for use in DI containers. Read only.
		/// </summary>
		public static Type StateManagerType => typeof(AppStateManager);

		/// <summary>
		/// Creates a <see cref="IAppStateService"/> instance.
		/// </summary>
		public static IAppStateService CreateStateManager(SynchronizationContext syncContext, IAppStateControllerFactory controllerFactory, IAppViewFactory viewFactory, IServiceProvider serviceProvider)
		{
			if (controllerFactory == null)
			{
				throw new ArgumentNullException(nameof(controllerFactory));
			}

			if (viewFactory == null)
			{
				throw new ArgumentNullException(nameof(viewFactory));
			}

			if (serviceProvider == null)
			{
				throw new ArgumentNullException(nameof(serviceProvider));
			}

			return new AppStateManager(syncContext, controllerFactory, viewFactory, serviceProvider);
		}

		/// <summary>
		/// Creates a <see cref="IAppStateService"/> instance.
		/// </summary>
		public static IAppStateService CreateStateManager(SynchronizationContext syncContext, IAppViewFactory viewFactory, IServiceProvider serviceProvider)
		{
			if (viewFactory == null)
			{
				throw new ArgumentNullException(nameof(viewFactory));
			}

			if (serviceProvider == null)
			{
				throw new ArgumentNullException(nameof(serviceProvider));
			}

			var controllerFactory = new AppStateControllerFactory();

			return new AppStateManager(syncContext, controllerFactory, viewFactory, serviceProvider);
		}

		/// <summary>
		/// Creates a <see cref="IAppStateService"/> instance.
		/// </summary>
		public static IAppStateService CreateStateManager(IAppViewFactory viewFactory, IServiceProvider serviceProvider)
		{
			if (viewFactory == null)
			{
				throw new ArgumentNullException(nameof(viewFactory));
			}

			if (serviceProvider == null)
			{
				throw new ArgumentNullException(nameof(serviceProvider));
			}

			var controllerFactory = new AppStateControllerFactory();

			return new AppStateManager(controllerFactory, viewFactory, serviceProvider);
		}

		/// <summary>
		/// Creates a <see cref="IAppViewService"/> instance.
		/// </summary>
		public static IAppViewService CreateViewManager(GameObject go)
		{
			if (go == null)
			{
				throw new ArgumentNullException(nameof(go));
			}

			return go.AddComponent<AppViewService>();
		}
	}
}
