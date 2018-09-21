// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Seetings of a <see cref="IAppStateService"/> instance.
	/// </summary>
	public interface IAppStateServiceSettings
	{
		/// <summary>
		/// Gets or sets trace switch used by the <see cref="TraceSource"/> instance.
		/// </summary>
		SourceSwitch TraceSwitch { get; set; }

		/// <summary>
		/// Gets a collection of trace listeners attached to the <see cref="TraceSource"/> used for logging.
		/// </summary>
		TraceListenerCollection TraceListeners { get; }

		/// <summary>
		/// Gets or sets maximum allowed number of simultanous stack operations. Default value is 0 (no limits).
		/// </summary>
		int MaxNumberOfPendingOperations { get; set; }
	}
}
