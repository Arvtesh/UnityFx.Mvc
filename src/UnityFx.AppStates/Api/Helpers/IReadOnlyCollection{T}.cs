// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;

namespace UnityFx.AppStates
{
#if NET35

	/// <summary>
	/// Represents a strongly-typed, read-only collection of elements.
	/// </summary>
	public interface IReadOnlyCollection<T> : IEnumerable<T>
	{
		/// <summary>
		/// Gets the number of elements in the collection.
		/// </summary>
		/// <value>The number of elements in the collection.</value>
		int Count { get; }
	}

#endif
}
