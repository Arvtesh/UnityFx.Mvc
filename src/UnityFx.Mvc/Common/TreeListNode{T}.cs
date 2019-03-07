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
		/// Gets the node children.
		/// </summary>
		public IEnumerable<T> GetChildren()
		{
			return new ChildEnumerable(this, false);
		}

		/// <summary>
		/// Gets the node children recursively.
		/// </summary>
		public IEnumerable<T> GetChildrenRecursive()
		{
			return new ChildEnumerable(this, true);
		}

		/// <summary>
		/// Checks whether the specified node is a child of this one.
		/// </summary>
		/// <param name="other">The node in question.</param>
		/// <returns>Returns <see langword="true"/> if the specified node is child of this one; <see langword="false"/> otherwise.</returns>
		public bool IsChildOf(TreeListNode<T> other)
		{
			if (other != null)
			{
				var nodeParent = _parent;

				if (nodeParent == other)
				{
					return true;
				}
				else if (nodeParent != null)
				{
					return nodeParent.IsChildOf(other);
				}
			}

			return false;
		}

		#endregion

		#region implementation

		private class ChildEnumerable : IEnumerable<T>
		{
			private readonly TreeListNode<T> _parent;
			private readonly bool _recursive;

			internal ChildEnumerable(TreeListNode<T> first, bool recursive)
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
