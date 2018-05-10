// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A factory for <see cref="AppView"/> instances.
	/// </summary>
	/// <seealso cref="AppView"/>
	public interface IAppViewService
	{
		/// <summary>
		/// Gets child views.
		/// </summary>
		AppViewCollection Views { get; }

		/// <summary>
		/// Initiates an animated transition from <paramref name="fromView"/> to <paramref name="toView"/>.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="fromView"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="toView"/> is <see langword="null"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the service is disposed.</exception>
		IAsyncOperation PlayTransition(AppView fromView, AppView toView);

		/// <summary>
		/// Initiates a present animation for the <paramref name="view"/> specified.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="view"/> is <see langword="null"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the service is disposed.</exception>
		IAsyncOperation PlayPresentTransition(AppView view);

		/// <summary>
		/// Initiates a dismiss animation for the <paramref name="view"/> specified.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="view"/> is <see langword="null"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the service is disposed.</exception>
		IAsyncOperation PlayDismissTransition(AppView view);

		/// <summary>
		/// Creates an empty view with the specified <paramref name="id"/> on top of the <paramref name="insertAfter"/> one.
		/// If <paramref name="insertAfter"/> is <see langword="null"/> the view is created below all others.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> is <see langword="null"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the service is disposed.</exception>
		AppView CreateView(string id, AppView insertAfter, AppViewOptions options);

		/// <summary>
		/// Creates an empty view with the specified <paramref name="id"/> on top of the <paramref name="parent"/> one.
		/// If <paramref name="parent"/> is <see langword="null"/> the view is created below all others.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> is <see langword="null"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the service is disposed.</exception>
		AppView CreateChildView(string id, AppView parent, AppViewOptions options);
	}
}
