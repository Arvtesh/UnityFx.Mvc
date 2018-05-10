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
	public class AppStateCollection : IReadOnlyCollection<AppState>
	{
		#region data

		private AppState _first;
		private AppState _last;
		private int _count;

		#endregion

		#region interface

		/// <summary>
		/// Returns top element of the stack.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the collection is empty.</exception>
		public AppState Peek()
		{
			ThrowIfEmpty();
			return _last;
		}

		/// <summary>
		/// Attempts to get top element of the stack.
		/// </summary>
		public bool TryPeek(out AppState result)
		{
			result = _last;
			return result != null;
		}

		/// <summary>
		/// Copies the collection content to an array.
		/// </summary>
		public void CopyTo(AppState[] states)
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
				cur = cur.NextState;
			}
		}

		/// <summary>
		/// Copies the collection content to another collection.
		/// </summary>
		public void CopyTo(ICollection<AppState> states)
		{
			if (states == null)
			{
				throw new ArgumentNullException(nameof(states));
			}

			var cur = _first;

			while (cur != null)
			{
				states.Add(cur);
				cur = cur.NextState;
			}
		}

		/// <summary>
		/// Returns the collection content as an array.
		/// </summary>
		public AppState[] ToArray()
		{
			if (_count > 0)
			{
				var result = new AppState[_count];
				var cur = _first;
				var index = 0;

				while (cur != null)
				{
					result[index++] = cur;
					cur = cur.NextState;
				}

				return result;
			}
			else
			{
#if NET35
				return new AppState[0];
#else
				return Array.Empty<AppState>();
#endif
			}
		}

		#endregion

		#region internals

		internal AppState First => _first;
		internal AppState Last => _last;

		internal void Add(AppState state)
		{
			Debug.Assert(state != null);

			if (_first != null)
			{
				Debug.Assert(_last != null);
				Debug.Assert(_count > 0);

				_last.NextState = state;
				state.PrevState = _last;
				++_count;
			}
			else
			{
				_first = state;
				_last = state;
				_count = 1;
			}
		}

		internal void AddFirst(AppState state)
		{
			Debug.Assert(state != null);

			if (_first != null)
			{
				Debug.Assert(_last != null);
				Debug.Assert(_count > 0);

				_first.PrevState = state;
				state.NextState = _first;
				_first = state;
				++_count;
			}
			else
			{
				_first = state;
				_last = state;
				_count = 1;
			}
		}

		internal void AddLast(AppState state)
		{
			Debug.Assert(state != null);

			if (_last != null)
			{
				Debug.Assert(_first != null);
				Debug.Assert(_count > 0);

				_last.NextState = state;
				state.PrevState = _last;
				_last = state;
				++_count;
			}
			else
			{
				_first = state;
				_last = state;
				_count = 1;
			}
		}

		internal void Add(AppState state, AppState insertAfter)
		{
			Debug.Assert(state != null);

			if (insertAfter != null)
			{
				insertAfter.NextState = state;
				state.PrevState = insertAfter;

				if (insertAfter == _last)
				{
					_last = state;
				}

				++_count;
			}
			else
			{
				AddFirst(state);
			}
		}

		internal void Remove(AppState state)
		{
			if (state != null)
			{
				var prev = state.PrevState;
				var next = state.NextState;

				if (prev != null)
				{
					prev.NextState = next;
				}

				if (next != null)
				{
					next.PrevState = prev;
				}

				state.PrevState = null;
				state.NextState = null;

				if (_first == state)
				{
					_first = next;
				}

				if (_last == state)
				{
					_last = prev;
				}

				--_count;
			}
		}

		internal void Clear()
		{
			var cur = _first;

			while (cur != null)
			{
				var next = cur.NextState;
				cur.NextState = null;
				cur.PrevState = null;
				cur = next;
			}

			_first = null;
			_last = null;
			_count = 0;
		}

		internal Enumerable GetEnumerableLifo()
		{
			return new Enumerable(_last, false);
		}

		internal AppState[] ToArrayLifo()
		{
			if (_count > 0)
			{
				var result = new AppState[_count];
				var cur = _last;
				var index = 0;

				while (cur != null)
				{
					result[index++] = cur;
					cur = cur.PrevState;
				}

				return result;
			}
			else
			{
#if NET35
				return new AppState[0];
#else
				return Array.Empty<AppState>();
#endif
			}
		}

		#endregion

		#region IReadOnlyColection

		/// <inheritdoc/>
		public int Count => _count;

		#endregion

		#region IEnumerable

		/// <summary>
		/// App states enumerable.
		/// </summary>
		public struct Enumerable : IEnumerable<AppState>
		{
			private AppState _first;
			private bool _fwd;

			internal Enumerable(AppState first, bool forward)
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
			IEnumerator<AppState> IEnumerable<AppState>.GetEnumerator()
			{
				throw new NotImplementedException();
			}

			/// <inheritdoc/>
			IEnumerator IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// App states enumerator.
		/// </summary>
		public struct Enumerator : IEnumerator<AppState>
		{
			private AppState _first;
			private AppState _current;
			private bool _fwd;

			/// <inheritdoc/>
			public AppState Current => _current;

			/// <inheritdoc/>
			object IEnumerator.Current => _current;

			internal Enumerator(AppState first, bool forward)
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
					}
					else
					{
						_current = _fwd ? _current.NextState : _current.PrevState;

						if (_current == null)
						{
							_first = null;
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
		IEnumerator<AppState> IEnumerable<AppState>.GetEnumerator() => new Enumerator(_first, true);

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
