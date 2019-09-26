// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;

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
		Task<IView> CreateViewAsync(Type controllerType, int zIndex);
	}
}
