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
	public interface IAppStateView : IAppView
	{
		/// <summary>
		/// Returns <see langword="true"/> if the view should cover all screen (views under it are not visible);
		/// <see langword="false"/> otherwise. Read only.
		/// </summary>
		/// <seealso cref="SetExclusive(bool)"/>
		bool IsExclusive { get; }

		/// <summary>
		/// Sets exclusive mode for the view.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown if the view has been disposed.</exception>
		/// <seealso cref="IsExclusive"/>
		void SetExclusive(bool exclusive);

		/// <summary>
		/// Adds an existing <see cref="GameObject"/> to the specified layer.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="go"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if invalid <paramref name="layer"/> is specified.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the view has been disposed.</exception>
		void Add(GameObject go, int layer);
	}
}
