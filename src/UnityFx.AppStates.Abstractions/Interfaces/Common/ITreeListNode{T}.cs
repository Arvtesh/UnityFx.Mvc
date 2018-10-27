// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic node of linked list.
	/// </summary>
	/// <typeparam name="T">Type of the node.</typeparam>
	/// <seealso cref="ITreeListCollection{T}"/>
	/// <seealso cref="ITreeListNodeAccess{T}"/>
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
	}
}
