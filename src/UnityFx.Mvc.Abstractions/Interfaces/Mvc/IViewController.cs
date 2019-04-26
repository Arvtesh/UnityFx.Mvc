// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A generic view controller.
	/// </summary>
	/// <remarks>
	/// As the name states, main responsibility of a view controller is managing its view. While <see cref="View"/> can be loaded
	/// and unloaded multiple times during the controller lifetime, typical use-case is loading view at the controller constructor
	/// and unloading it when the controller is disposed. Disposing a controller typically disposes the attached view.
	/// </remarks>
	/// <seealso cref="IView"/>
	/// <seealso cref="IPresenter"/>
	/// <seealso cref="IPresentContext"/>
	/// <seealso cref="IPresentService"/>
	/// <seealso cref="IViewControllerFactory"/>
	/// <seealso cref="IViewController{TView}"/>
	/// <seealso href="https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93controller"/>
	public interface IViewController : ICommandTarget, IDisposable
	{
		/// <summary>
		/// Raised when the controller <see cref="View"/> has been loaded.
		/// </summary>
		/// <seealso cref="View"/>
		/// <seealso cref="IsViewLoaded"/>
		/// <seealso cref="LoadViewAsync"/>
		event EventHandler<AsyncCompletedEventArgs> LoadViewCompleted;

		/// <summary>
		/// Gets a value indicating whether the <see cref="View"/> can be safely used.
		/// </summary>
		/// <seealso cref="View"/>
		/// <seealso cref="LoadViewAsync"/>
		/// <seealso cref="UnloadView"/>
		bool IsViewLoaded { get; }

		/// <summary>
		/// Gets a view managed by the controller. Returns <see langword="null"/> if the view is not loaded.
		/// </summary>
		/// <remarks>
		/// Implementation may decide to lazy-load its view on first access. In this case the property would never return <see langword="null"/>.
		/// </remarks>
		/// <seealso cref="IsViewLoaded"/>
		/// <seealso cref="LoadViewAsync"/>
		/// <seealso cref="UnloadView"/>
		IView View { get; }

		/// <summary>
		/// Loads <see cref="View"/>. If view is already loaded (or another load operation is already running) the method returns immediately.
		/// </summary>
		/// <remarks>
		/// Implementation may decide to load views asynchronously. In this case the method just initiates the operation and returns.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if unload operation is pending.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the controller is disposed.</exception>
		/// <seealso cref="View"/>
		/// <seealso cref="IsViewLoaded"/>
		/// <seealso cref="LoadViewCompleted"/>
		/// <seealso cref="UnloadView"/>
		void LoadViewAsync();

		/// <summary>
		/// Disposes the view.
		/// </summary>
		/// <seealso cref="View"/>
		/// <seealso cref="IsViewLoaded"/>
		/// <seealso cref="LoadViewAsync"/>
		void UnloadView();
	}
}
