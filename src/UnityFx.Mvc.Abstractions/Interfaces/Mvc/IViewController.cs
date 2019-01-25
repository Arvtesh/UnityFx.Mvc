// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A generic view controller.
	/// </summary>
	/// <seealso cref="IView"/>
	/// <seealso href="https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93controller"/>
	public interface IViewController : ICommandTarget, IDisposable
	{
		/// <summary>
		/// Raised when <see cref="LoadViewAsync"/> is completed.
		/// </summary>
		event EventHandler<AsyncCompletedEventArgs> LoadViewCompleted;

		/// <summary>
		/// Gets the controller name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets a view managed by the controller.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if the view is not loaded.</exception>
		/// <seealso cref="IsViewLoaded"/>
		/// <seealso cref="LoadViewAsync"/>
		/// <seealso cref="UnloadView"/>
		IView View { get; }

		/// <summary>
		/// Gets a value indicating whether the <see cref="View"/> can be safely used.
		/// </summary>
		/// <seealso cref="View"/>
		bool IsViewLoaded { get; }

		/// <summary>
		/// Initiates loading <see cref="View"/> if it is not loaded yet.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown if the controller is disposed.</exception>
		/// <seealso cref="LoadViewCompleted"/>
		/// <seealso cref="View"/>
		void LoadViewAsync();

		/// <summary>
		/// Unloads the view (if any).
		/// </summary>
		/// <seealso cref="LoadViewAsync"/>
		/// <seealso cref="View"/>
		void UnloadView();
	}
}
