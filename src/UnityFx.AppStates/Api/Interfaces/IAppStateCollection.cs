// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A read-only collection of <see cref="IAppState"/>.
	/// </summary>
	/// <seealso cref="IAppState"/>
#if NET35
	public interface IAppStateCollection : ICollection<IAppState>
#else
	public interface IAppStateCollection : IReadOnlyCollection<IAppState>
#endif
	{
		/// <summary>
		/// Returns last element of the collection.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the collection is empty.</exception>
		IAppState Peek();

		/// <summary>
		/// Attempts to get last element of the collection.
		/// </summary>
		bool TryPeek(out IAppState result);
	}
}
