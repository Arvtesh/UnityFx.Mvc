// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;

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
		public static IAppStateService CreateStateManager(SynchronizationContext syncContext, IAppStateControllerFactory controllerFactory, IAppStateViewFactory viewFactory, IServiceProvider serviceProvider)
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
		public static IAppStateService CreateStateManager(SynchronizationContext syncContext, IAppStateViewFactory viewFactory, IServiceProvider serviceProvider)
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
		public static IAppStateService CreateStateManager(IAppStateViewFactory viewFactory, IServiceProvider serviceProvider)
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
	}
}
