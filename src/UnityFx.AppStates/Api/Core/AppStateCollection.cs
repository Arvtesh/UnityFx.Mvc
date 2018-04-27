// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A collection of states.
	/// </summary>
	public class AppStateCollection : IReadOnlyCollection<AppState>
	{
		#region data

		private static AppStateCollection _empty = new AppStateCollection();

		private List<AppState> _states;

		#endregion

		#region interface

		/// <summary>
		/// Gets an empty states collection.
		/// </summary>
		public static AppStateCollection Empty => _empty;

		/// <summary>
		/// Returns top element of the stack.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the collection is empty.</exception>
		public AppState Peek()
		{
			ThrowIfEmpty();
			return _states[_states.Count - 1];
		}

		/// <summary>
		/// Attempts to get top element of the stack.
		/// </summary>
		public bool TryPeek(out AppState result)
		{
			if (_states != null)
			{
				var childCount = _states.Count;

				if (childCount > 0)
				{
					result = _states[childCount - 1];
					return true;
				}
			}

			result = null;
			return false;
		}

		/// <summary>
		/// Enumerates the collectin elements in LIFO order.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<AppState> GetEnumerableLifo()
		{
			var n = _states?.Count - 1 ?? -1;

			while (n >= 0)
			{
				yield return _states[n--];
			}
		}

		/// <summary>
		/// Copies the collection content to an array.
		/// </summary>
		public void CopyTo(AppState[] states)
		{
			_states?.CopyTo(states);
		}

		/// <summary>
		/// Copies the collection content to an array.
		/// </summary>
		public void CopyTo(AppState[] states, int arrayIndex)
		{
			_states?.CopyTo(states, arrayIndex);
		}

		/// <summary>
		/// Copies the collection content to another collection.
		/// </summary>
		public void CopyTo(ICollection<AppState> states)
		{
			if (_states != null)
			{
				foreach (var state in _states)
				{
					states.Add(state);
				}
			}
		}

		/// <summary>
		/// Returns the collection content as an array.
		/// </summary>
		public AppState[] ToArray()
		{
#if NET35
			return _states?.ToArray() ?? new AppState[0];
#else
			return _states?.ToArray() ?? Array.Empty<AppState>();
#endif
		}

		/// <summary>
		/// Returns the collection content as LIFO array.
		/// </summary>
		public AppState[] ToArrayLifo()
		{
			var childCount = _states?.Count ?? 0;
			var result = new AppState[childCount];

			for (var i = 0; i < childCount; ++i)
			{
				result[i] = _states[childCount - i - 1];
			}

			return result;
		}

		#endregion

		#region internals

		internal AppState this[int index] => _states[index];

		internal void Add(AppState state)
		{
			if (_states == null)
			{
				_states = new List<AppState>();
			}

			_states.Add(state);
		}

		internal void Remove(AppState state)
		{
			if (_states != null && _states.Remove(state))
			{
				// ...
			}
		}

		internal void Clear()
		{
			_states?.Clear();
		}

		#endregion

		#region IReadOnlyColection

		/// <inheritdoc/>
		public int Count => _states?.Count ?? 0;

		#endregion

		#region IEnumerable

		/// <inheritdoc/>
		public List<AppState>.Enumerator GetEnumerator()
		{
			return _states?.GetEnumerator() ?? new List<AppState>.Enumerator();
		}

		/// <inheritdoc/>
		IEnumerator<AppState> IEnumerable<AppState>.GetEnumerator()
		{
			return _states?.GetEnumerator() ?? Enumerable.Empty<AppState>().GetEnumerator();
		}

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return (this as IEnumerable<AppState>).GetEnumerator();
		}

		#endregion

		#region implementation

		private void ThrowIfEmpty()
		{
			if (_states == null || _states.Count == 0)
			{
				throw new InvalidOperationException("The stack is empty.");
			}
		}

		#endregion
	}
}
