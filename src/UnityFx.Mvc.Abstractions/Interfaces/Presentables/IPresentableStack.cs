// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A stack of <see cref="IPresentable"/> items.
	/// </summary>
	/// <seealso cref="IPresentable"/>
	/// <seealso cref="IPresentService"/>
#if NET35
	public interface IPresentableStack : IEnumerable<IPresentable>
#else
	public interface IPresentableStack : IReadOnlyCollection<IPresentable>
#endif
	{
#if NET35
		/// <summary>
		/// Gets the number of elements in the collection.
		/// </summary>
		/// <value>
		/// The number of elements in the collection.
		/// </value>
		int Count { get; }
#endif

		/// <summary>
		/// Gets top element of the stack.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the collection is empty.</exception>
		IPresentable Peek();

		/// <summary>
		/// Attempts to gets top element of the stack.
		/// </summary>
		/// <param name="result">Top element of the stack.</param>
		/// <returns>Returns <see langword="true"/> if the operation succeeds; <see langword="false"/> otherwise.</returns>
		bool TryPeek(out IPresentable result);
	}
}
