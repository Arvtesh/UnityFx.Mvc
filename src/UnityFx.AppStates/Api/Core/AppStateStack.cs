// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A LIFO collection of states.
	/// </summary>
	public class AppStateStack : IReadOnlyCollection<AppState>
	{
		#region data

		private List<AppState> _states;

		#endregion

		#region interface

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
		/// Returns the collection content as array.
		/// </summary>
		public AppState[] ToArray()
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
		public IEnumerator<AppState> GetEnumerator()
		{
			var n = _states?.Count - 1 ?? -1;

			while (n >= 0)
			{
				yield return _states[n--];
			}
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
