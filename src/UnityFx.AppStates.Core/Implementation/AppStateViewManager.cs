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
	/// Implementatino of <see cref="IAppStateViewFactory"/>.
	/// </summary>
	internal class AppStateViewManager : MonoBehaviour, IAppStateViewManager, IReadOnlyList<IAppStateView>
	{
		#region data
		#endregion

		#region interface
		#endregion

		#region MonoBehaviour
		#endregion

		#region IAppStateViewManager

		public IReadOnlyList<IAppStateView> Views => this;

		#endregion

		#region IAppStateViewFactory

		public IAppStateView CreateView(string name, IAppStateView insertAfter)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("Invalid view name", nameof(name));
			}

			var viewGo = new GameObject(name);
			var viewTransform = viewGo.transform;
			viewTransform.SetParent(transform, false);

			if (insertAfter is AppStateView iav)
			{
				var siblingIndex = iav.transform.GetSiblingIndex();
				viewTransform.SetSiblingIndex(siblingIndex + 1);
			}

			return viewGo.AddComponent<AppStateView>();
		}

		#endregion

		#region IReadOnlyList

		public IAppStateView this[int index] => transform.GetChild(index).GetComponent<IAppStateView>();

		#endregion

		#region IReadOnlyCollection

		public int Count => transform.childCount;

		#endregion

		#region IEnumerable

		public IEnumerator<IAppStateView> GetEnumerator() => GetEnumeratorInternal();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumeratorInternal();

		#endregion

		#region implementation

		private IEnumerator<IAppStateView> GetEnumeratorInternal()
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				yield return transform.GetChild(i).GetComponent<IAppStateView>();
			}
		}

		#endregion
	}
}
