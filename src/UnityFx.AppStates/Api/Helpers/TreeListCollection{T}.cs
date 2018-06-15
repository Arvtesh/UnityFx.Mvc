// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A collection of states.
	/// </summary>
	public class TreeListCollection<T> : ICollection<T>, IReadOnlyCollection<T> where T : TreeListNode<T>
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
		/// Returns top element of the stack.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the collection is empty.</exception>
		public T Peek()
		{
			ThrowIfEmpty();
			return _last;
		}

		/// <summary>
		/// Attempts to get top element of the stack.
		/// </summary>
		public bool TryPeek(out T result)
		{
			result = _last;
			return result != null;
		}

		/// <summary>
		/// Copies the collection content to an array.
		/// </summary>
		public void CopyTo(T[] states)
		{
			if (states == null)
			{
				throw new ArgumentNullException(nameof(states));
			}

			if (states.Length != _count)
			{
				throw new InvalidOperationException();
			}

			var cur = _first;
			var index = 0;

			while (cur != null)
			{
				states[index++] = cur;
				cur = cur.Next;
			}
		}

		/// <summary>
		/// Copies the collection content to another collection.
		/// </summary>
		public void CopyTo(ICollection<T> states)
		{
			if (states == null)
			{
				throw new ArgumentNullException(nameof(states));
			}

			var cur = _first;

			while (cur != null)
			{
				states.Add(cur);
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

		#endregion

		#region internals

		internal void AddFirst(T node)
		{
			Debug.Assert(node != null);

			if (_first != null)
			{
				Debug.Assert(_last != null);
				Debug.Assert(_count > 0);

				_first.Prev = node;
				node.Next = _first;
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

		internal void AddLast(T node)
		{
			Debug.Assert(node != null);

			if (_last != null)
			{
				Debug.Assert(_first != null);
				Debug.Assert(_count > 0);

				_last.Next = node;
				node.Prev = _last;
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

		internal void Add(T node, T insertAfter)
		{
			Debug.Assert(node != null);

			if (insertAfter != null)
			{
				insertAfter.Next = node;
				node.Prev = insertAfter;

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

		internal Enumerable GetEnumerableLifo()
		{
			return new Enumerable(_last, false);
		}

		internal T[] ToArrayLifo()
		{
			if (_count > 0)
			{
				var result = new T[_count];
				var cur = _last;
				var index = 0;

				while (cur != null)
				{
					result[index++] = cur;
					cur = cur.Prev;
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

		#endregion

		#region ICollection

		/// <inheritdoc/>
		public bool IsReadOnly => false;

		/// <inheritdoc/>
		public void Add(T node)
		{
			Debug.Assert(node != null);

			if (_first != null)
			{
				Debug.Assert(_last != null);
				Debug.Assert(_count > 0);

				_last.Next = node;
				node.Prev = _last;
				++_count;
			}
			else
			{
				_first = node;
				_last = node;
				_count = 1;
			}
		}

		/// <inheritdoc/>
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
						prev.Next = next;
					}

					if (next != null)
					{
						next.Prev = prev;
					}

					node.Prev = null;
					node.Next = null;

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
			}

			return false;
		}

		/// <inheritdoc/>
		public void Clear()
		{
			var cur = _first;

			while (cur != null)
			{
				var next = cur.Next;
				cur.Next = null;
				cur.Prev = null;
				cur = next;
			}

			_first = null;
			_last = null;
			_count = 0;
		}

		/// <inheritdoc/>
		public bool Contains(T item)
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

		#region IReadOnlyColection

		/// <inheritdoc/>
		public int Count => _count;

		#endregion

		#region IEnumerable

		/// <summary>
		/// Items enumerable.
		/// </summary>
		public struct Enumerable : IEnumerable<T>
		{
			private T _first;
			private bool _fwd;

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
			private T _first;
			private T _current;
			private bool _fwd;

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

		/// <inheritdoc/>
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
				throw new InvalidOperationException("The stack is empty.");
			}
		}

		#endregion
	}
}
