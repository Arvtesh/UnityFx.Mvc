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
	internal class TreeListNode<T> : ITreeListNode<T> where T : class, ITreeListNode<T>
	{
		#region data

		private readonly T _parentNode;

		#endregion

		#region interface

		protected TreeListNode()
		{
		}

		protected TreeListNode(T parentNode)
		{
			_parentNode = parentNode;
		}

		#endregion

		#region ITreeListNode

		public T Parent => _parentNode;
		public T Prev { get; internal set; }
		public T Next { get; internal set; }
		public IEnumerable<T> Children => new ChildEnumerable(this as T);

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
