// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A generic node of linked list.
	/// </summary>
	/// <typeparam name="T">Type of the node.</typeparam>
	/// <seealso cref="TreeListCollection{T}"/>
	internal class TreeListNode<T> where T : TreeListNode<T>
	{
		#region data

		private readonly T _parent;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeListNode{T}"/> class.
		/// </summary>
		protected TreeListNode()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeListNode{T}"/> class.
		/// </summary>
		protected TreeListNode(T parent)
		{
			_parent = parent;
		}

		/// <summary>
		/// Gets parent node.
		/// </summary>
		public T Parent => _parent;

		/// <summary>
		/// Gets or sets previous sibling node.
		/// </summary>
		public T Prev { get; set; }

		/// <summary>
		/// Gets or sets next sibling node.
		/// </summary>
		public T Next { get; set; }

		#endregion
	}
}
