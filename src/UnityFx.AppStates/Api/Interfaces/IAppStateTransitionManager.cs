// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A view transition controller.
	/// </summary>
	public interface IAppStateTransitionManager
	{
		/// <summary>
		/// tt
		/// </summary>
		/// <param name="fromView"></param>
		/// <param name="toView"></param>
		/// <returns></returns>
		IAsyncOperation PlayTransition(AppView fromView, AppView toView);

		/// <summary>
		/// tt
		/// </summary>
		/// <param name="view">A view being pushed.</param>
		/// <returns></returns>
		IAsyncOperation PlayPresentTransition(AppView view);

		/// <summary>
		/// tt
		/// </summary>
		/// <param name="view">A view being popped.</param>
		/// <returns></returns>
		IAsyncOperation PlayDismissTransition(AppView view);
	}
}
