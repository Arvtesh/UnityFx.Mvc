// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityFx.Mvc
{
	internal class PresentableProxy<TController> : PresentableProxy, IPresentResult<TController> where TController : IPresentable
	{
		#region data
		#endregion

		#region interface

		public PresentableProxy(PresentService presentManager, PresentableProxy parent, Type controllerType, PresentArgs args)
			: base(presentManager, parent, controllerType, args)
		{
		}

		#endregion

		#region IPresentResult

		public new TController Controller => (TController)base.Controller;

		#endregion
	}
}
