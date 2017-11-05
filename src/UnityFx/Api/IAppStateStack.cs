// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;

namespace UnityFx.App
{
	/// <summary>
	/// Returns a stack of the <see cref="IAppState"/> instances.
	/// </summary>
	public interface IAppStateStack : IReadOnlyCollection<IAppState>
	{
		/// <summary>
		/// Returns the top state. Note that the top state is not always the active one.
		/// </summary>
		IAppState Peek();
	}
}
