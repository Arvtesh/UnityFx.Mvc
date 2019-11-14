// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A factory of <see cref="IView"/> instances.
	/// </summary>
	/// <seealso cref="IView"/>
	public interface IViewFactory
	{
		/// <summary>
		/// Creates a view for the specified controller.
		/// </summary>
		/// <param name="controllerType">Type of the view controller.</param>
		/// <param name="zIndex">Z-order index.</param>
		/// <param name="options">Present options.</param>
		/// <param name="parent">Parent transform for the view (or <see langword="null"/>).</param>
		Task<IView> CreateViewAsync(Type controllerType, int zIndex, PresentOptions options, Transform parent);
	}
}
