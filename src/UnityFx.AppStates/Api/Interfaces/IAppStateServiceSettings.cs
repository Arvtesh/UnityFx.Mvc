// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Settings of a <see cref="IAppStateService"/>.
	/// </summary>
	/// <seealso cref="IAppStateService"/>
	public interface IAppStateServiceSettings
	{
		/// <summary>
		/// Gets or sets trace switch used by the <see cref="TraceSource"/> instance.
		/// </summary>
		SourceSwitch TraceSwitch { get; set; }

		/// <summary>
		/// Gets a collection of <see cref="TraceListener"/> instances attached to the <see cref="TraceSource"/> used for logging.
		/// </summary>
		TraceListenerCollection TraceListeners { get; }

		/// <summary>
		/// Gets or sets maximum allowed number of simultanous stack operations.
		/// </summary>
		int MaxNumberOfPendingOperations { get; set; }

		/// <summary>
		/// Gets or sets domain name that is used to construct deeplinks to states managed by the service.
		/// </summary>
		string DeeplinkDomain { get; set; }

		/// <summary>
		/// Gets or sets scheme name that is used to construct deeplinks to states managed by the service.
		/// </summary>
		string DeeplinkScheme { get; set; }
	}
}
