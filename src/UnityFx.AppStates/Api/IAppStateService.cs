// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.App
{
	/// <summary>
	/// A generic application state manager.
	/// </summary>
	/// <seealso cref="IAppState"/>
	public interface IAppStateService : IAppStateManager, IDisposable
	{
	}
}
