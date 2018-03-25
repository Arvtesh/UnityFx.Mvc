// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic application state service.
	/// </summary>
	/// <threadsafety static="true" instance="false"/>
	/// <seealso cref="AppState"/>
	public interface IAppStateService : IAppStateManager
	{
		/// <summary>
		/// Gets the service settings.
		/// </summary>
		IAppStateServiceSettings Settings { get; }
	}
}
