// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;

namespace UnityFx.App
{
	/// <summary>
	/// 
	/// </summary>
	public static class AppStates
	{
		/// <summary>
		/// 
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

			return new AppStateManager(SynchronizationContext.Current, viewFactory, serviceProvider);
		}
	}
}
