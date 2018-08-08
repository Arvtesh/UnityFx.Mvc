// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Seetings of a <see cref="IAppStateService"/> instance.
	/// </summary>
	public interface IAppStateServiceSettings
	{
		/// <summary>
		/// Gets or sets maximum allowed number of simultanous stack operations.
		/// </summary>
		int MaxNumberOfPendingOperations { get; set; }
	}
}
