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
		/// Gets the object index in the list.
		/// </summary>
		protected int GetIndex()
		{
			var result = 0;
			var prev = Prev;

			while (prev != null)
			{
				++result;
				prev = prev.Prev;
			}

			return result;
		}

		/// <summary>
		/// Gets the node children.
		/// </summary>
		public ChildEnumerable GetChildren()
		{
			return new ChildEnumerable(this as T, false);
		}

		/// <summary>
		/// Gets the node children recursively.
		/// </summary>
		public ChildEnumerable GetChildrenRecursive()
		{
			return new ChildEnumerable(this as T, true);
		}

		/// <summary>
		/// Checks whether the specified node is a child of this one.
		/// </summary>
		/// <param name="other">The node in question.</param>
		/// <returns>Returns <see langword="true"/> if the specified node is child of this one; <see langword="false"/> otherwise.</returns>
		public bool IsChildOf(T other)
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

		public struct ChildEnumerator : IEnumerator<T>
		{
			private readonly T _parent;
			private readonly bool _recursive;
			private T _current;

			internal ChildEnumerator(T first, bool recursive)
			{
				_parent = first;
				_recursive = recursive;
				_current = first;
			}

			public T Current => _current;

			object IEnumerator.Current => _current;

			public bool MoveNext()
			{
				if (_current != null)
				{
					_current = _current.Next;

					while (_current != null)
					{
						if (_recursive)
						{
							if (_current.IsChildOf(_parent))
							{
								return true;
							}
						}
						else if (_current.Parent == _parent)
						{
							return true;
						}

						_current = _current.Next;
					}
				}

				return false;
			}

			public void Reset()
			{
				_current = _parent;
			}

			public void Dispose()
			{
			}
		}

		public struct ChildEnumerable : IEnumerable<T>
		{
			private readonly T _parent;
			private readonly bool _recursive;

			internal ChildEnumerable(T first, bool recursive)
			{
				_parent = first;
				_recursive = recursive;
			}

			public ChildEnumerator GetEnumerator()
			{
				return new ChildEnumerator(_parent, _recursive);
			}

			IEnumerator<T> IEnumerable<T>.GetEnumerator()
			{
				return new ChildEnumerator(_parent, _recursive);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return new ChildEnumerator(_parent, _recursive);
			}
		}

		#endregion
	}
}
