// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Seetings of a <see cref="IAppStateService"/> instance.
	/// </summary>
	public interface IAppStateServiceConfig
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
		/// Adds a new configuration for an application's present pipeline.
		/// </summary>
		/// <param name="predicate"></param>
		IPresentPipelineBuilder AddBuilder(Predicate<IPresentContext> predicate);
	}
}
