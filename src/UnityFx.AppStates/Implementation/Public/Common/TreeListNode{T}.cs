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
	}
}
