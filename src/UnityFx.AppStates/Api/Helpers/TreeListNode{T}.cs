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
	/// <seealso cref="TreeListCollection{T}"/>
	public class TreeListNode<T> : ITreeListNode<T> where T : class, ITreeListNode<T>
	{
		#region data

		private readonly T _parentNode;

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
		protected TreeListNode(T parentNode)
		{
			_parentNode = parentNode;
		}

		#endregion

		#region ITreeListNode

		/// <summary>
		/// Gets a parent node for this one (if any).
		/// </summary>
		public T Parent => _parentNode;

		/// <summary>
		/// Gets previous sibling node (if any).
		/// </summary>
		public T Prev { get; internal set; }

		/// <summary>
		/// Gets next sibling node (if any).
		/// </summary>
		public T Next { get; internal set; }

		/// <summary>
		/// Gets the node children.
		/// </summary>
		public IEnumerable<T> Children => new ChildEnumerable(this as T);

		/// <summary>
		/// Checks whether this node is a child of another.
		/// </summary>
		/// <param name="node">The node to check.</param>
		/// <returns>Returns <see langword="true"/> if this node is a child of the one passed with argument; otherwise, <see langword="false"/>.</returns>
		public bool IsChildOf(T node)
		{
			if (node != null && _parentNode != null && _parentNode != node)
			{
				return _parentNode.IsChildOf(node);
			}

			return false;
		}

		#endregion

		#region implementation

		private class ChildEnumerable : IEnumerable<T>
		{
			private T _parent;

			internal ChildEnumerable(T first)
			{
				_parent = first;
			}

			public IEnumerator<T> GetEnumerator()
			{
				var cur = _parent.Next;

				while (cur != null)
				{
					if (cur.IsChildOf(_parent))
					{
						yield return cur;
					}

					cur = cur.Next;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return (this as IEnumerable<T>).GetEnumerator();
			}
		}

		#endregion
	}
}
