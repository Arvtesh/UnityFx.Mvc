// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityFx.AppStates.Common
{
	/// <summary>
	/// A generic linked list read-only collection.
	/// </summary>
	/// <typeparam name="T">Type of the collectino items.</typeparam>
	/// <seealso cref="ITreeListNode{T}"/>
	/// <seealso cref="ITreeListNodeAccess{T}"/>
#if NET35
	public interface ITreeListCollection<T> : ICollection<T> where T : ITreeListNode<T>
#else
	public interface ITreeListCollection<T> : IReadOnlyCollection<T> where T : ITreeListNode<T>
#endif
	{
		/// <summary>
		/// Returns last element of the collection.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the collection is empty.</exception>
		T Peek();

		/// <summary>
		/// Attempts to get last element of the collection.
		/// </summary>
		bool TryPeek(out T result);
	}
}
