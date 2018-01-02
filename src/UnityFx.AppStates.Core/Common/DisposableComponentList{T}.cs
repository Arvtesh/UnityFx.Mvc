// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A disposable component collection.
	/// </summary>
	internal class DisposableComponentList<T> : DisposableMonoBehaviour, IReadOnlyList<T>
	{
		#region data
		#endregion

		#region interface
		#endregion

		#region MonoBehaviour

		private void OnDestroy()
		{
			SetDisposed();
		}

		#endregion

		#region IReadOnlyList

		public T this[int index]
		{
			get
			{
				ThrowIfDisposed();
				return transform.GetChild(index).GetComponent<T>();
			}
		}

		#endregion

		#region IReadOnlyCollection

		public int Count
		{
			get
			{
				ThrowIfDisposed();
				return transform.childCount;
			}
		}

		#endregion

		#region IEnumerable

		public IEnumerator<T> GetEnumerator() => GetEnumeratorInternal();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

		#endregion

		#region implementation

		private IEnumerator<T> GetEnumeratorInternal()
		{
			ThrowIfDisposed();

			for (var i = 0; i < transform.childCount; i++)
			{
				yield return transform.GetChild(i).GetComponent<T>();
			}
		}

		#endregion
	}
}
