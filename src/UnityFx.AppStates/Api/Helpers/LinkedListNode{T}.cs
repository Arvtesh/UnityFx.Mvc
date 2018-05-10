// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityFx.AppStates
{
	/// <summary>
	/// tt
	/// </summary>
	/// <typeparam name="T">Type of the node.</typeparam>
	public class LinkedListNode<T> where T : class
	{
		#region data

		private readonly T _parent;

		#endregion

		#region interface

		/// <summary>
		/// Gets a parent node for this one (if any).
		/// </summary>
		public T Parent => _parent;

		/// <summary>
		/// Gets previous sibling node (if any).
		/// </summary>
		public T Prev { get; internal set; }

		/// <summary>
		/// Gets next sibling node (if any).
		/// </summary>
		public T Next { get; internal set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="LinkedListNode{T}"/> class.
		/// </summary>
		protected LinkedListNode()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LinkedListNode{T}"/> class.
		/// </summary>
		protected LinkedListNode(T parent)
		{
			_parent = parent;
		}

		#endregion
	}
}
