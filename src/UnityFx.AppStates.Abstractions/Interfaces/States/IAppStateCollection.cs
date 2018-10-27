// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A read-only collection of <see cref="IAppState"/>.
	/// </summary>
	/// <seealso cref="IAppState"/>
	public interface IAppStateCollection : ITreeListCollection<IAppState>
	{
	}
}
