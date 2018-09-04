// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityFx.AppStates.Common
{
	/// <summary>
	/// A generic node of linked list.
	/// </summary>
	/// <typeparam name="T">Type of the node.</typeparam>
	/// <seealso cref="TreeListCollection{T}"/>
	public class TreeListNode<T> : ITreeListNode<T>, ITreeListNodeAccess<T> where T : class, ITreeListNode<T>
	{
		#region data

		private readonly T _parentNode;
		private T _next;
		private T _prev;

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
		/// Gets parent node.
		/// </summary>
		public T Parent => _parentNode;

		/// <summary>
		/// Gets previous sibling node.
		/// </summary>
		public T Prev => _prev;

		/// <summary>
		/// Gets next sibling node.
		/// </summary>
		public T Next => _next;

		/// <summary>
		/// Gets the node children.
		/// </summary>
		public IEnumerable<T> Children => new ChildEnumerable(this as T, false);

		/// <summary>
		/// Gets the node children recursively.
		/// </summary>
		public IEnumerable<T> ChildrenRecursive => new ChildEnumerable(this as T, true);

		/// <summary>
		/// Checks whether the specified node is a child of this one.
		/// </summary>
		/// <param name="node">The node in question.</param>
		/// <returns>Returns <see langword="true"/> if the specified node is child of this one; <see langword="false"/> otherwise.</returns>
		public bool IsChildOf(T node)
		{
			if (node != null && _parentNode != null && _parentNode != node)
			{
				return _parentNode.IsChildOf(node);
			}

			return false;
		}

		#endregion

		#region ITreeListNodeAccess

		/// <inheritdoc/>
		void ITreeListNodeAccess<T>.SetNext(T next)
		{
			_next = next;
		}

		/// <inheritdoc/>
		void ITreeListNodeAccess<T>.SetPrev(T prev)
		{
			_prev = prev;
		}

		#endregion

		#region implementation

		private class ChildEnumerable : IEnumerable<T>
		{
			private readonly T _parent;
			private readonly bool _recursive;

			internal ChildEnumerable(T first, bool recursive)
			{
				_parent = first;
				_recursive = recursive;
			}

			public IEnumerator<T> GetEnumerator()
			{
				var cur = _parent.Next;

				if (_recursive)
				{
					while (cur != null)
					{
						if (cur.IsChildOf(_parent))
						{
							yield return cur;
						}

						cur = cur.Next;
					}
				}
				else
				{
					while (cur != null)
					{
						if (cur.Parent == _parent)
						{
							yield return cur;
						}

						cur = cur.Next;
					}
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
