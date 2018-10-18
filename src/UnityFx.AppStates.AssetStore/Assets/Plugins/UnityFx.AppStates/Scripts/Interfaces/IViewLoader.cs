// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A factory of <see cref="IView"/> instances.
	/// </summary>
	public interface IViewLoader
	{
		/// <summary>
		/// Asynchronously loads the specified view.
		/// </summary>
		/// <param name="resourceId">Identifier of the resource to load.</param>
		/// <param name="parent">A <see cref="Transform"/> to attach the prefab instance loaded to.</param>
		IAsyncOperation<IView> LoadViewAsync(string resourceId, Transform parent);
	}
}
