// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
#if UNITYFX_SUPPORT_TAP
using System.Threading.Tasks;
#endif

namespace UnityFx.AppStates
{
	/// <summary>
	/// tt
	/// </summary>
	public class AppStateService : AppStateManager
	{
		#region data
		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="AppStateService"/> class.
		/// </summary>
		/// <param name="syncContext"></param>
		/// <param name="viewManager"></param>
		/// <param name="services"></param>
		/// <param name="controllerFactory"></param>
		public AppStateService(
			SynchronizationContext syncContext,
			IAppStateControllerFactory controllerFactory,
			IAppStateViewFactory viewManager,
			IServiceProvider services)
			: base(syncContext, controllerFactory, viewManager, services)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppStateService"/> class.
		/// </summary>
		/// <param name="syncContext"></param>
		/// <param name="viewManager"></param>
		/// <param name="services"></param>
		public AppStateService(
			SynchronizationContext syncContext,
			IAppStateViewFactory viewManager,
			IServiceProvider services)
			: this(syncContext, new AppStateControllerFactory(services), viewManager, services)
		{
		}

		#endregion

		#region IAppStateService

		/// <inheritdoc/>
		public IAppStateServiceSettings Settings => Shared;

		#endregion
	}
}
