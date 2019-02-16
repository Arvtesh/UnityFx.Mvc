// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityFx.Mvc
{
	internal class ViewControllerProxy<TController> : ViewControllerProxy, IPresentResult<TController> where TController : IViewController
	{
		#region data
		#endregion

		#region interface

		public ViewControllerProxy(PresentService presentManager, ViewControllerProxy parent, Type controllerType, PresentArgs args)
			: base(presentManager, parent, controllerType, args)
		{
		}

		#endregion

		#region IPresentResult

		public new TController Controller => (TController)base.Controller;

		#endregion
	}
}
