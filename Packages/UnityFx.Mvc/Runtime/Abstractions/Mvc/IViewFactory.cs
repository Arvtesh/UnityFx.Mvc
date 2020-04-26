// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Provides methods for creation and disposal of views.
	/// </summary>
	/// <seealso cref="IView"/>
	public interface IViewFactory
	{
		/// <summary>
		/// Loads view prefab.
		/// </summary>
		/// <param name="resourceId">Resource identifier of the view prefab.</param>
		Task<GameObject> LoadViewPrefabAsync(string resourceId);

		/// <summary>
		/// Creates a view for a controller of the specified type.
		/// </summary>
		/// <param name="resourceId">Resource identifier of the view prefab.</param>
		/// <param name="layer"></param>
		/// <param name="zIndex">Z-order index.</param>
		/// <param name="options">Present options.</param>
		/// <param name="parent">Parent transform for the view (or <see langword="null"/>).</param>
		Task<IView> PresentViewAsync(string resourceId, int layer, int zIndex, ViewControllerFlags flags, Transform parent);
	}
}
