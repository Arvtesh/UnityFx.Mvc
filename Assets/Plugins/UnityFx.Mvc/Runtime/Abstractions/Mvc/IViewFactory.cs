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
		/// <param name="controllerType">Type of the view controller to create view for.</param>
		/// <param name="zIndex">Z-order index.</param>
		/// <param name="options">Present options.</param>
		/// <param name="parent">Parent transform for the view (or <see langword="null"/>).</param>
		Task<IView> CreateAsync(Type controllerType, int zIndex, PresentOptions options, Transform parent);
	}
}
