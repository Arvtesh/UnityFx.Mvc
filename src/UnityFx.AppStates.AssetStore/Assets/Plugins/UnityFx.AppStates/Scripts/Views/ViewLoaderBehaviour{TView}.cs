// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using UnityEngine;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Implementation of <see cref="IViewLoader"/>.
	/// </summary>
	public abstract class ViewLoaderBehaviour<TView> : MonoBehaviour, IViewLoader where TView : Component, IView
	{
		#region data
		#endregion

		#region interface

		/// <summary>
		/// Loads a <see cref="GameObject"/> for the specified view.
		/// </summary>
		/// <remarks>
		/// Method is expected to use <paramref name="cs"/> to notify the opertion completion.
		/// </remarks>
		/// <param name="resourceId">Resource identifier to load.</param>
		/// <param name="parent">A <see cref="Transform"/> to attach the prefab instance loaded to.</param>
		/// <param name="cs">Completion source of the corresponsing async operation.</param>
		protected abstract IEnumerator LoadView(string resourceId, Transform parent, IAsyncCompletionSource<IView> cs);

		#endregion

		#region MonoBehaviour
		#endregion

		#region IViewLoader

		/// <summary>
		/// Asynchronously loads the specified prefab.
		/// </summary>
		/// <param name="resourceId">Identifier of the resource to load.</param>
		/// <param name="parent">A <see cref="Transform"/> to attach the prefab instance loaded to.</param>
		public IAsyncOperation<IView> LoadViewAsync(string resourceId, Transform parent)
		{
			if (resourceId == null)
			{
				throw new ArgumentNullException("resourceId");
			}

			if (resourceId == string.Empty)
			{
				throw new ArgumentException("Resource identifier cannot be an empty string.", "resourceId");
			}

			var result = new AsyncCompletionSource<IView>(AsyncOperationStatus.Running);
			StartCoroutine(LoadView(resourceId, parent, result));
			return result;
		}

		#endregion

		#region implementation
		#endregion
	}
}
