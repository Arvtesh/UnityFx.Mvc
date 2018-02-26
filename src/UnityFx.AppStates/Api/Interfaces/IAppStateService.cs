// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
#if UNITYFX_SUPPORT_TAP
using System.Threading.Tasks;
#endif
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic application state service.
	/// </summary>
	/// <threadsafety static="true" instance="false"/>
	/// <seealso cref="IAppState"/>
	public interface IAppStateService : IAppStateManager
	{
		/// <summary>
		/// Gets the service settings.
		/// </summary>
		IAppStateServiceSettings Settings { get; }
	}
}
