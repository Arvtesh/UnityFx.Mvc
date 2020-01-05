// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A collection of <see cref="IViewController"/> items.
	/// </summary>
	/// <seealso cref="IViewController"/>
	public interface IViewControllerCollection : IReadOnlyCollection<IViewController>
	{
		/// <summary>
		/// Gets top element of the stack.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the collection is empty.</exception>
		IViewController Peek();

		/// <summary>
		/// Attempts to gets top element of the stack.
		/// </summary>
		/// <param name="result">Top element of the stack.</param>
		/// <returns>Returns <see langword="true"/> if the operation succeeds; <see langword="false"/> otherwise.</returns>
		bool TryPeek(out IViewController result);
	}
}
