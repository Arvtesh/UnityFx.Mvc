// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic view transition factory.
	/// </summary>
	/// <seealso cref="AppView"/>
	public interface IAppViewTransitionFactory
	{
		/// <summary>
		/// Initiates an animated transition from <paramref name="fromView"/> to <paramref name="toView"/>.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="toView"/> is <see langword="null"/>.</exception>
		IAsyncOperation PlayPresentTransition(IAppView fromView, IAppView toView, bool replace);

		/// <summary>
		/// Initiates a dismiss animation for the <paramref name="view"/> specified.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="view"/> is <see langword="null"/>.</exception>
		IAsyncOperation PlayDismissTransition(IAppView view, IAppView toView);
	}
}
