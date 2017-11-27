// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;

namespace UnityFx.App
{
	/// <summary>
	/// Settings of a <see cref="IAppStateService"/>.
	/// </summary>
	/// <seealso cref="IAppStateService"/>
	public interface IAppStateServiceSettings
	{
		/// <summary>
		/// 
		/// </summary>
		SourceSwitch TraceSwitch { get; set; }

		/// <summary>
		/// 
		/// </summary>
		TraceListenerCollection TraceListeners { get; }
	}
}
