// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic tree list collection.
	/// </summary>
	/// <seealso cref="TreeListNode{T}"/>
	public class TreeListCollection<T> : ITreeListCollection<T>, ICollection<T> where T : class, ITreeListNode<T>
	{
		#region data

		private T _first;
		private T _last;
		private int _count;

		#endregion

		#region interface

		/// <summary>
		/// Gets the first element.
		/// </summary>
		public T First => _first;

		/// <summary>
		/// Gets the last element.
		/// </summary>
		public T Last => _last;

		/// <summary>
		/// Adds a new node to the collection.
		/// </summary>
		public void Add(T node)
		{
			if (node == null)
			{
				throw new ArgumentNullException(nameof(node));
			}

			if (_first != null)
			{
				Debug.Assert(_last != null);
				Debug.Assert(_count > 0);

				SetLink(_last, node);

				_last = node;
				++_count;
			}
			else
			{
				_first = node;
				_last = node;
				_count = 1;
			}
		}

		/// <summary>
		/// Adds a new node to the collection.
		/// </summary>
		public void AddFirst(T node)
		{
			if (node == null)
			{
				throw new ArgumentNullException(nameof(node));
			}

			if (_first != null)
			{
				Debug.Assert(_last != null);
				Debug.Assert(_count > 0);

				SetLink(node, _first);

				_first = node;
				++_count;
			}
			else
			{
				_first = node;
				_last = node;
				_count = 1;
			}
		}

		/// <summary>
		/// Adds a new node to the collection.
		/// </summary>
		public void AddLast(T node)
		{
			if (node == null)
			{
				throw new ArgumentNullException(nameof(node));
			}

			if (_last != null)
			{
				Debug.Assert(_first != null);
				Debug.Assert(_count > 0);

				SetLink(_last, node);

				_last = node;
				++_count;
			}
			else
			{
				_first = node;
				_last = node;
				_count = 1;
			}
		}

		/// <summary>
		/// Adds a new node to the collection.
		/// </summary>
		public void Add(T node, T insertAfter)
		{
			if (insertAfter != null)
			{
				if (node == null)
				{
					throw new ArgumentNullException(nameof(node));
				}

				Debug.Assert(Contains(insertAfter));
				SetLink(insertAfter, node);

				if (insertAfter == _last)
				{
					_last = node;
				}

				++_count;
			}
			else
			{
				AddFirst(node);
			}
		}

		/// <summary>
		/// Removes a specific node from the collection.
		/// </summary>
		public bool Remove(T node)
		{
			if (node != null)
			{
				var prev = node.Prev;
				var next = node.Next;

				if (prev != null || next != null)
				{
					if (prev != null)
					{
						SetNext(prev, next);
					}

					if (next != null)
					{
						SetPrev(next, prev);
					}

					Unlink(node);

					if (_first == node)
					{
						_first = next;
					}

					if (_last == node)
					{
						_last = prev;
					}

					--_count;

					return true;
				}
				else if (_first == node)
				{
					Debug.Assert(_last == node);
					Debug.Assert(_count == 1);

					Unlink(node);

					_first = null;
					_last = null;
					_count = 0;

					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Removes all nodes from the collection.
		/// </summary>
		public void Clear()
		{
			var cur = _first;

			while (cur != null)
			{
				var next = cur.Next;
				Unlink(cur);
				cur = next;
			}

			_first = null;
			_last = null;
			_count = 0;
		}

		/// <summary>
		/// Copies the collection content to an array.
		/// </summary>
		public void CopyTo(T[] array)
		{
			if (array == null)
			{
				throw new ArgumentNullException(nameof(array));
			}

			if (array.Length != _count)
			{
				throw new InvalidOperationException();
			}

			var cur = _first;
			var index = 0;

			while (cur != null)
			{
				array[index++] = cur;
				cur = cur.Next;
			}
		}

		/// <summary>
		/// Copies the collection content to another collection.
		/// </summary>
		public void CopyTo(ICollection<T> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException(nameof(other));
			}

			if (other.IsReadOnly)
			{
				throw new NotSupportedException();
			}

			var cur = _first;

			while (cur != null)
			{
				other.Add(cur);
				cur = cur.Next;
			}
		}

		/// <summary>
		/// Returns the collection content as an array.
		/// </summary>
		public T[] ToArray()
		{
			if (_count > 0)
			{
				var result = new T[_count];
				var cur = _first;
				var index = 0;

				while (cur != null)
				{
					result[index++] = cur;
					cur = cur.Next;
				}

				return result;
			}
			else
			{
#if NET35
				return new T[0];
#else
				return Array.Empty<T>();
#endif
			}
		}

		/// <summary>
		/// Returns an enumerable that iterates through the collection starting from the last element.
		/// </summary>
		public Enumerable Reverse() => new Enumerable(_last, false);

		#endregion

		#region ITreeListCollection

		/// <inheritdoc/>
		public T Peek()
		{
			ThrowIfEmpty();
			return _last;
		}

		/// <inheritdoc/>
		public bool TryPeek(out T result)
		{
			result = _last;
			return result != null;
		}

		#endregion

		#region ICollection

		/// <inheritdoc/>
		public int Count => _count;

		/// <inheritdoc/>
		bool ICollection<T>.IsReadOnly => true;

		/// <inheritdoc/>
		void ICollection<T>.Add(T node)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc/>
		bool ICollection<T>.Remove(T node)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc/>
		void ICollection<T>.Clear()
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc/>
		public bool Contains(T item)
		{
			if (item != null)
			{
				var cur = _first;

				while (cur != null)
				{
					if (cur == item)
					{
						return true;
					}

					cur = cur.Next;
				}
			}

			return false;
		}

		/// <inheritdoc/>
		public void CopyTo(T[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException(nameof(array));
			}

			if (array.Length >= arrayIndex + _count)
			{
				throw new InvalidOperationException();
			}

			var cur = _first;
			var index = 0;

			while (cur != null)
			{
				array[index++] = cur;
				cur = cur.Next;
			}
		}

		#endregion

		#region IEnumerable

		/// <summary>
		/// Items enumerable.
		/// </summary>
		public struct Enumerable : IEnumerable<T>
		{
			private readonly T _first;
			private readonly bool _fwd;

			internal Enumerable(T first, bool forward)
			{
				_first = first;
				_fwd = forward;
			}

			/// <inheritdoc/>
			public Enumerator GetEnumerator()
			{
				return new Enumerator(_first, _fwd);
			}

			/// <inheritdoc/>
			IEnumerator<T> IEnumerable<T>.GetEnumerator()
			{
				return new Enumerator(_first, _fwd);
			}

			/// <inheritdoc/>
			IEnumerator IEnumerable.GetEnumerator()
			{
				return new Enumerator(_first, _fwd);
			}
		}

		/// <summary>
		/// Items enumerator.
		/// </summary>
		public struct Enumerator : IEnumerator<T>
		{
			private readonly bool _fwd;

			private T _first;
			private T _current;

			/// <inheritdoc/>
			public T Current => _current;

			/// <inheritdoc/>
			object IEnumerator.Current => _current;

			internal Enumerator(T first, bool forward)
			{
				_first = first;
				_current = null;
				_fwd = forward;
			}

			/// <inheritdoc/>
			public bool MoveNext()
			{
				if (_first != null)
				{
					if (_current == null)
					{
						_current = _first;
						return true;
					}
					else
					{
						_current = _fwd ? _current.Next : _current.Prev;

						if (_current == null)
						{
							_first = null;
						}
						else
						{
							return true;
						}
					}
				}

				return false;
			}

			/// <inheritdoc/>
			public void Reset()
			{
				throw new NotSupportedException();
			}

			/// <inheritdoc/>
			public void Dispose()
			{
				_first = null;
				_current = null;
			}
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		public Enumerator GetEnumerator() => new Enumerator(_first, true);

		/// <inheritdoc/>
		IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator(_first, true);

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => new Enumerator(_first, true);

		#endregion

		#region implementation

		private void ThrowIfEmpty()
		{
			if (_count == 0)
			{
				throw new InvalidOperationException("The collection is empty.");
			}
		}

		private static void SetNext(T node, T next)
		{
			Debug.Assert(node != null);
			(node as ITreeListNodeAccess<T>).SetNext(next);
		}

		private static void SetPrev(T node, T prev)
		{
			Debug.Assert(node != null);
			(node as ITreeListNodeAccess<T>).SetPrev(prev);
		}

		private static void Unlink(T node)
		{
			Debug.Assert(node != null);

			var n = node as ITreeListNodeAccess<T>;
			n.SetNext(null);
			n.SetPrev(null);
		}

		private static void SetLink(T prev, T next)
		{
			Debug.Assert(next != null);
			Debug.Assert(prev != null);

			(next as ITreeListNodeAccess<T>).SetPrev(prev);
			(prev as ITreeListNodeAccess<T>).SetNext(next);
		}

		#endregion
	}
}
