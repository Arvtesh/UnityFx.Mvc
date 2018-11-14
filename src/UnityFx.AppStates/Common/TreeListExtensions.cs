// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Extensions for <see cref="TreeListNode{T}"/> and <see cref="TreeListCollection{T}"/>.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static class TreeListExtensions
	{
		#region TreeListNode

		/// <summary>
		/// Gets the node children.
		/// </summary>
		public static IEnumerable<T> GetChildren<T>(this T node) where T : TreeListNode<T>
		{
			return new ChildEnumerable<T>(node, false);
		}

		/// <summary>
		/// Gets the node children recursively.
		/// </summary>
		public static IEnumerable<T> GetChildrenRecursive<T>(this T node) where T : TreeListNode<T>
		{
			return new ChildEnumerable<T>(node, true);
		}

		/// <summary>
		/// Checks whether the specified node is a child of this one.
		/// </summary>
		/// <param name="node">The owner node.</param>
		/// <param name="other">The node in question.</param>
		/// <returns>Returns <see langword="true"/> if the specified node is child of this one; <see langword="false"/> otherwise.</returns>
		public static bool IsChildOf<T>(this T node, T other) where T : TreeListNode<T>
		{
			if (other != null)
			{
				var nodeParent = node.Parent;

				if (nodeParent == other)
				{
					return true;
				}
				else if (nodeParent != null)
				{
					return IsChildOf(nodeParent, other);
				}
			}

			return false;
		}

		#endregion

		#region implementation

		private class ChildEnumerable<T> : IEnumerable<T> where T : TreeListNode<T>
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
