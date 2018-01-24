// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic application view.
	/// </summary>
	/// <seealso cref="IAppStateViewFactory"/>
	public interface IAppView : IEnumerable<GameObject>, IDisposable
	{
		/// <summary>
		/// Returns name of the view. Read only.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Returns view bounds (in world space) based on its content. Read only.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown if the view has been disposed.</exception>
		Bounds Bounds { get; }

		/// <summary>
		/// Returns <see langword="true"/> if the view is enabled (enabled views are visible, disabled ones
		/// are neither visible nor interactable); <see langword="false"/> otherwise. Read only.
		/// </summary>
		/// <remarks>
		/// If value of the property is <see langword="true"/> the view still may not be visible depending
		/// on its location in the view hierarchy.
		/// </remarks>
		/// <seealso cref="SetEnabled(bool)"/>
		/// <seealso cref="IsInteractable"/>
		bool IsEnabled { get; }

		/// <summary>
		/// Returns <see langword="true"/> if input processing is enabled for the view; <see langword="false"/>
		/// otherwise. Note that disabled views do not process input. Read only.
		/// </summary>
		/// <seealso cref="SetInteractable(bool)"/>
		/// <seealso cref="IsEnabled"/>
		bool IsInteractable { get; }

		/// <summary>
		/// Enables or disables the view.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown if the view has been disposed.</exception>
		/// <seealso cref="IsEnabled"/>
		void SetEnabled(bool enabled);

		/// <summary>
		/// Enables or disables input processing for the view.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown if the view has been disposed.</exception>
		/// <seealso cref="IsInteractable"/>
		void SetInteractable(bool inputEnabled);

		/// <summary>
		/// Adds an existing <see cref="GameObject"/> to the view.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="go"/> is <see langword="null"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the view has been disposed.</exception>
		/// <seealso cref="Remove(GameObject)"/>
		/// <seealso cref="Clear"/>
		void Add(GameObject go);

		/// <summary>
		/// Removes the specified <see cref="GameObject"/> from the view and destroys it.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown if the view has been disposed.</exception>
		/// <seealso cref="Add(GameObject)"/>
		/// <seealso cref="Clear"/>
		bool Remove(GameObject go);

		/// <summary>
		/// Clears the view content.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown if the view has been disposed.</exception>
		/// <seealso cref="Remove(GameObject)"/>
		void Clear();
	}
}
