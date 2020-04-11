// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Provides bindings from a controller to its view.
	/// </summary>
	/// <seealso cref="IViewController"/>
	/// <seealso cref="IView"/>
	public interface IViewControllerBindings
	{
		/// <summary>
		/// Gets view binding for the controller specified.
		/// </summary>
		string GetViewPath(Type controllerType);
	}
}
