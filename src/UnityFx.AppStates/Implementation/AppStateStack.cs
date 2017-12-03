// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.App
{
	/// <summary>
	/// Implementation of <see cref="IAppStateStack"/>.
	/// </summary>
	internal sealed class AppStateStack : IAppStateStack
	{
		#region data

		private readonly Transform _rootTransform;

		#endregion

		#region interface

		public AppStateStack(Transform t)
		{
			_rootTransform = t;
		}

		public IAppStateInternal this[int index] => GetState(index);

		public IEnumerator<IAppStateInternal> GetEnumerator() => GetEnumeratorInternal();

		public IAppStateInternal GetState(int index)
		{
			return _rootTransform.GetChild(index).GetComponent<IAppStateInternal>();
		}

		public bool TryPeekEx(out IAppStateInternal result)
		{
			var childCount = _rootTransform.childCount;

			if (childCount > 0)
			{
				result = GetState(childCount - 1);
				return result != null;
			}

			result = null;
			return false;
		}

		public IAppStateInternal[] ToArray()
		{
			var childCount = _rootTransform.childCount;
			var result = new IAppStateInternal[childCount];

			for (var i = 0; i < childCount; ++i)
			{
				result[i] = GetState(childCount - i - 1);
			}

			return result;
		}

		#endregion

		#region IAppStateStack

		public IAppState Peek()
		{
			ThrowIfEmpty();
			return GetState(_rootTransform.childCount - 1);
		}

		public bool TryPeek(out IAppState result)
		{
			var childCount = _rootTransform.childCount;

			if (childCount > 0)
			{
				result = GetState(childCount - 1);
				return result != null;
			}

			result = null;
			return false;
		}

		public bool Contains(IAppState state)
		{
			foreach (Transform t in _rootTransform)
			{
				if (t.GetComponent<IAppState>() == state)
				{
					return true;
				}
			}

			return false;
		}

		#endregion

		#region IReadOnlyColection

		public int Count => _rootTransform.childCount;

		#endregion

		#region IEnumerable

		IEnumerator<IAppState> IEnumerable<IAppState>.GetEnumerator() => GetEnumeratorInternal();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

		#endregion

		#region implementation

		public IEnumerator<IAppStateInternal> GetEnumeratorInternal()
		{
			var n = _rootTransform.childCount - 1;

			while (n >= 0)
			{
				yield return GetState(n--);
			}
		}

		public void ThrowIfEmpty()
		{
			if (_rootTransform.childCount == 0)
			{
				throw new InvalidOperationException("The stack is empty.");
			}
		}

		#endregion
	}
}
