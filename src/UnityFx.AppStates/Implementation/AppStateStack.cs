// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Implementation of <see cref="IAppStateStack"/>.
	/// </summary>
	internal sealed class AppStateStack : IAppStateStack
	{
		#region data

		private List<AppState> _states;

		#endregion

		#region interface

		public AppState this[int index] => _states[index];

		public IEnumerator<AppState> GetEnumerator()
		{
			var n = _states?.Count - 1 ?? -1;

			while (n >= 0)
			{
				yield return _states[n--];
			}
		}

		public void Add(AppState state)
		{
			if (_states == null)
			{
				_states = new List<AppState>();
			}

			_states.Add(state);
		}

		public void Remove(AppState state)
		{
			if (_states != null && _states.Remove(state))
			{
				// ...
			}
		}

		public bool TryPeek(out AppState result)
		{
			if (_states != null)
			{
				var childCount = _states.Count;

				if (childCount > 0)
				{
					result = _states[childCount - 1];
					return result != null;
				}
			}

			result = null;
			return false;
		}

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

		public void Clear()
		{
			_states?.Clear();
		}

		#endregion

		#region IAppStateStack

		IAppState IAppStateStack.Peek()
		{
			ThrowIfEmpty();
			return _states[_states.Count - 1];
		}

		bool IAppStateStack.TryPeek(out IAppState result)
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

		#endregion

		#region IReadOnlyColection

		public int Count => _states?.Count ?? 0;

		#endregion

		#region IEnumerable

		IEnumerator<IAppState> IEnumerable<IAppState>.GetEnumerator()
		{
			var n = _states?.Count - 1 ?? -1;

			while (n >= 0)
			{
				yield return _states[n--];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (this as IEnumerable<IAppState>).GetEnumerator();
		}

		#endregion

		#region implementation

		public void ThrowIfEmpty()
		{
			if (_states == null || _states.Count == 0)
			{
				throw new InvalidOperationException("The stack is empty.");
			}
		}

		#endregion
	}
}
