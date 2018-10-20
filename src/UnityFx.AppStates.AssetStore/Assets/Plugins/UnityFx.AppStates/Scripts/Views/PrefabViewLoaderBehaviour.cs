// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using UnityEngine;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Implementation of <see cref="IViewLoader"/> that loads views from <see cref="Resources"/>.
	/// </summary>
	public class PrefabViewLoaderBehaviour : ViewLoaderBehaviour<PrefabViewBehaviour>
	{
		#region data
		#endregion

		#region interface

		/// <summary>
		/// Attempts to complete the load operation using <paramref name="go"/> as result.
		/// </summary>
		/// <param name="go">The prefab loaded.</param>
		/// <param name="parent">A <see cref="Transform"/> to attach the <paramref name="go"/> to.</param>
		/// <param name="cs">Completion source of the corresponsing async operation.</param>
		/// <seealso cref="LoadView(string, Transform, IAsyncCompletionSource{IView})"/>
		protected static void SetResult(GameObject go, Transform parent, IAsyncCompletionSource<IView> cs)
		{
			if (go)
			{
				var resultGo = Instantiate(go, parent, false);
				var view = resultGo.GetComponent<IView>();

				if (view == null)
				{
					view = resultGo.AddComponent<PrefabViewBehaviour>();
				}

				cs.TrySetResult(view);
			}
			else
			{
				cs.TrySetException(new ArgumentNullException("go"));
			}
		}

		/// <summary>
		/// Loads a <see cref="GameObject"/> for the specified view. Default implementation attemts to load it from <see cref="Resources"/>.
		/// </summary>
		/// <remarks>
		/// Method is expected to call <see cref="SetResult(GameObject, Transform, IAsyncCompletionSource{IView})"/> when the view is
		/// successfully loaded. Otherwise it should use <paramref name="cs"/> to notify the opertion completion.
		/// </remarks>
		/// <param name="resourceId">Resource identifier to load.</param>
		/// <param name="parent">A <see cref="Transform"/> to attach the prefab instance loaded to.</param>
		/// <param name="cs">Completion source of the corresponsing async operation.</param>
		/// <seealso cref="SetResult(GameObject, Transform, IAsyncCompletionSource{IView})"/>
		protected override IEnumerator LoadView(string resourceId, Transform parent, IAsyncCompletionSource<IView> cs)
		{
			var op = Resources.LoadAsync(resourceId);
			yield return op;

			if (op.asset != null)
			{
				SetResult(op.asset as GameObject, parent, cs);
			}
			else
			{
				cs.TrySetException(string.Format("Resources.LoadAsync failed for view '{0}'.", resourceId));
			}
		}

		#endregion

		#region MonoBehaviour
		#endregion

		#region implementation
		#endregion
	}
}
