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
		/// Creates a view for a controller of the specified type.
		/// </summary>
		/// <param name="resourceId">Resource identifier of the view prefab.</param>
		/// <param name="parent">Parent transform for the view (or <see langword="null"/>).</param>
		/// <seealso cref="ReleaseView(IView)"/>
		Task<IView> CreateViewAsync(string resourceId, Transform parent);

		/// <summary>
		/// Releases the view.
		/// </summary>
		/// <param name="view">The view to release.</param>
		/// <seealso cref="CreateViewAsync(string, Transform)"/>
		void ReleaseView(IView view);
	}
}
