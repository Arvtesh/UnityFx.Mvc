// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Present delegate that is called right before presenting a controller.
	/// </summary>
	/// <param name="presenter">The presenter.</param>
	/// <param name="controllerInfo">Information about the controller being presented.</param>
	/// <param name="presentArgs">Controller present argument.</param>
	/// <seealso cref="IPresenter"/>
	/// <seealso cref="IPresenterBuilder"/>
	public delegate Task PresentDelegate(IPresentService presenter, IViewControllerInfo controllerInfo, PresentArgs presentArgs);
}
