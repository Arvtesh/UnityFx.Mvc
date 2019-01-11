// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Seetings of a <see cref="IPresentService"/> instance.
	/// </summary>
	public interface IPresentServiceSettings
	{
		/// <summary>
		/// Gets or sets trace switch used by the <see cref="TraceSource"/> instance.
		/// </summary>
		SourceSwitch TraceSwitch { get; set; }

		/// <summary>
		/// Gets a collection of trace listeners attached to the <see cref="TraceSource"/> used for logging.
		/// </summary>
		TraceListenerCollection TraceListeners { get; }
	}
}
