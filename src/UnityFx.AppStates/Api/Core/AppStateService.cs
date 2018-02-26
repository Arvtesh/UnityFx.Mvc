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
	public class AppStateService : AppStateManager, IAppStateServiceSettings
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
			IAppStateViewFactory viewManager,
			IServiceProvider services,
			IAppStateControllerFactory controllerFactory)
			: base(new TraceSource("AppStates"), syncContext, viewManager, services, controllerFactory)
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
			: this(syncContext, viewManager, services, new AppStateControllerFactory(services))
		{
		}

		#endregion

		#region IAppStateService

		/// <inheritdoc/>
		public IAppStateServiceSettings Settings => this;

		#endregion

		#region IAppStateServiceSettings

		/// <inheritdoc/>
		public SourceSwitch TraceSwitch { get => TraceSource.Switch; set => TraceSource.Switch = value; }

		/// <inheritdoc/>
		public TraceListenerCollection TraceListeners => TraceSource.Listeners;

		/// <inheritdoc/>
		public int MaxNumberOfPendingOperations { get; set; }

		/// <inheritdoc/>
		public string DeeplinkDomain { get; set; }

		/// <inheritdoc/>
		public string DeeplinkScheme { get; set; }

		#endregion
	}
}
