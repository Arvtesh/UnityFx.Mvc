// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace UnityFx.Mvc
{
	internal class ViewControllerProxy<TController> : ViewControllerProxy, IPresentResult<TController> where TController : IViewController
	{
		#region data
		#endregion

		#region interface

		public ViewControllerProxy(PresentService presenter, ViewControllerProxy parent, Type controllerType, PresentArgs args, int id)
			: base(presenter, parent, controllerType, args, id)
		{
		}

		#endregion

		#region IPresentResult

		public new TController Controller => (TController)base.Controller;

		public new Task<TController> PresentTask => throw new NotImplementedException();

		#endregion
	}
}
