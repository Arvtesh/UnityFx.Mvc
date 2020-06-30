// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Extensions of <see cref="IViewControllerCollection"/> interface.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class ViewControllerCollectionExtensions
	{
		/// <summary>
		/// Returns top element of the collection.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the collection is empty.</exception>
		public static IViewController Peek(this IViewControllerCollection viewControllers)
		{
			if (viewControllers.TryPeek(out var result))
			{
				return result;
			}

			throw new InvalidOperationException("The collection is empty.");
		}
	}
}
