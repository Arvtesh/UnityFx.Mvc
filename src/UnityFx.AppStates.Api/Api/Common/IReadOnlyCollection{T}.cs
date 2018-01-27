// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;

namespace UnityFx.AppStates
{
#if NET35

	/// <summary>
	/// tt
	/// </summary>
	public interface IReadOnlyCollection<T> : IEnumerable<T>
	{
		/// <summary>
		/// Returns the collection size. Read only.
		/// </summary>
		int Count { get; }
	}

#endif
}
