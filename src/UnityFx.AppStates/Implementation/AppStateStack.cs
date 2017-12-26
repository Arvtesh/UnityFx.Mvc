// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityFx.App
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

		public IEnumerator<AppState> GetEnumerator() => GetEnumeratorInternal();

		public void Add(AppState state)
		{
			if (_states == null)
			{
				_states = new List<AppState>();
			}

			_states.Add(state);

			StateAdded?.Invoke(this, new AppStateEventArgs(state));
		}

		public void Remove(AppState state)
		{
			if (_states != null && _states.Remove(state))
			{
				StateRemoved?.Invoke(this, new AppStateEventArgs(state));
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


		public AppState[] Release()
		{
			var result = ToArray();
			_states?.Clear();
			return result;
		}

		public void Clear()
		{
			_states?.Clear();
		}

		#endregion

		#region IAppStateStack

		public event EventHandler<AppStateEventArgs> StateAdded;
		public event EventHandler<AppStateEventArgs> StateRemoved;

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

		bool IAppStateStack.Contains(IAppState state)
		{
			if (_states != null)
			{
				foreach (var s in _states)
				{
					if (s == state)
					{
						return true;
					}
				}
			}

			return false;
		}

		#endregion

		#region IReadOnlyColection

		public int Count => _states?.Count ?? 0;

		#endregion

		#region IEnumerable

		IEnumerator<IAppState> IEnumerable<IAppState>.GetEnumerator() => GetEnumeratorInternal();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

		#endregion

		#region implementation

		public IEnumerator<AppState> GetEnumeratorInternal()
		{
			var n = _states?.Count - 1 ?? -1;

			while (n >= 0)
			{
				yield return _states[n--];
			}
		}

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
