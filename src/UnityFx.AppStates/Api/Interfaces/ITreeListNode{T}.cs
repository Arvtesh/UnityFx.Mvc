// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic node of linked list.
	/// </summary>
	/// <typeparam name="T">Type of the node.</typeparam>
	public interface ITreeListNode<T> where T : ITreeListNode<T>
	{
		/// <summary>
		/// Gets a parent node for this one (if any).
		/// </summary>
		T Parent { get; }

		/// <summary>
		/// Gets previous sibling node (if any).
		/// </summary>
		T Prev { get; }

		/// <summary>
		/// Gets next sibling node (if any).
		/// </summary>
		T Next { get; }

		/// <summary>
		/// Gets the node children.
		/// </summary>
		IEnumerable<T> Children { get; }

		/// <summary>
		/// Gets the node children.
		/// </summary>
		IEnumerable<T> ChildrenRecursive { get; }

		/// <summary>
		/// Checks whether this node is a child of another.
		/// </summary>
		/// <param name="node">The node to check.</param>
		/// <returns>Returns <see langword="true"/> if this node is a child of the one passed with argument; otherwise, <see langword="false"/>.</returns>
		bool IsChildOf(T node);
	}
}
