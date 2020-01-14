// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Present delegate that is called right before presenting a controller.
	/// </summary>
	/// <see cref="IPresenter"/>
	public delegate Task PresentDelegate(IPresentService presenter, IViewControllerInfo controllerInfo);
}
