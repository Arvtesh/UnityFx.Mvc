// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityFx.AppStates.Common
{
	/// <summary>
	/// A write-acess to <see cref="ITreeListNode{T}"/>.
	/// </summary>
	/// <typeparam name="T">Type of the node.</typeparam>
	/// <seealso cref="ITreeListCollection{T}"/>
	/// <seealso cref="ITreeListNode{T}"/>
	public interface ITreeListNodeAccess<T> where T : ITreeListNode<T>
	{
		/// <summary>
		/// Gets previous sibling node (if any).
		/// </summary>
		void SetPrev(T prev);

		/// <summary>
		/// Gets next sibling node (if any).
		/// </summary>
		void SetNext(T next);
	}
}
