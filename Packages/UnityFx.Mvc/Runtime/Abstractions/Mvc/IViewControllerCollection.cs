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
		/// Attempts to get top element of the collection.
		/// </summary>
		/// <param name="result">Top element of the collection.</param>
		/// <returns>Returns <see langword="true"/> if the collection contains at least one element; <see langword="false"/> otherwise.</returns>
		bool TryPeek(out IViewController result);

		/// <summary>
		/// Checks if a controller of the specified type is presented.
		/// </summary>
		bool Contains(Type controllerType);
	}
}
