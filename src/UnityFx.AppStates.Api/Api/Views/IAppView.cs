﻿// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic application view.
	/// </summary>
	/// <seealso cref="IAppViewFactory"/>
	/// <seealso cref="IAppViewLayer"/>
	/// <seealso cref="IAppViewService"/>
	public interface IAppView : IEnumerable<GameObject>, IDisposable
	{
		/// <summary>
		/// Returns name of the view. Read only.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Returns view content. Read only.
		/// </summary>
		ICollection<GameObject> Content { get; }

		/// <summary>
		/// Returns view bounds (in world space) based on its content. Read only.
		/// </summary>
		Bounds Bounds { get; }

		/// <summary>
		/// Returns <see langword="true"/> if the view should cover all screen (views under it are not visible);
		/// <see langword="false"/> otherwise. Read only.
		/// </summary>
		/// <seealso cref="SetExclusive(bool)"/>
		bool IsExclusive { get; }

		/// <summary>
		/// Returns <see langword="true"/> if the view is enabled (enabled views are visible, disabled ones
		/// are neither visible nor interactable); <see langword="false"/> otherwise. Read only.
		/// </summary>
		/// <remarks>
		/// If value of the property is <see langword="true"/> the view may or may not be visible. For example,
		/// if any view above this one has <see cref="IsExclusive"/> flag set the view will not be rendered.
		/// </remarks>
		/// <seealso cref="SetEnabled(bool)"/>
		/// <seealso cref="IsInteractable"/>
		/// <seealso cref="IsExclusive"/>
		bool IsEnabled { get; }

		/// <summary>
		/// Returns <see langword="true"/> if input processing is enabled for the view; <see langword="false"/>
		/// otherwise. Read only.
		/// </summary>
		/// <seealso cref="SetInteractable(bool)"/>
		/// <seealso cref="IsEnabled"/>
		bool IsInteractable { get; }

		/// <summary>
		/// Enables or disables the view.
		/// </summary>
		/// <seealso cref="IsEnabled"/>
		void SetEnabled(bool enabled);

		/// <summary>
		/// Enables or disables input processing for the view.
		/// </summary>
		/// <seealso cref="IsInteractable"/>
		void SetInteractable(bool inputEnabled);

		/// <summary>
		/// Sets exclusive mode for the view.
		/// </summary>
		/// <seealso cref="IsExclusive"/>
		void SetExclusive(bool exclusive);
	}
}