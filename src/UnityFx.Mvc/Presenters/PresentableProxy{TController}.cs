// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
#if !NET35
using UnityFx.Mvc.CompilerServices;
#endif

namespace UnityFx.Mvc
{
#if NET35
	internal class PresentableProxy<TController> : PresentableProxy, IPresentResult<TController> where TController : IPresentable
#else
	internal class PresentableProxy<TController> : PresentableProxy, IPresentResult<TController>, IPresentAwaiter<TController> where TController : IPresentable
#endif
	{
		#region data
		#endregion

		#region interface

		public PresentableProxy(PresentService presenter, PresentableProxy parent, Type controllerType, PresentArgs args, int id)
			: base(presenter, parent, controllerType, args, id)
		{
		}

		#endregion

		#region IPresentResult

		public new TController Controller => (TController)base.Controller;

#if !NET35

		public new IPresentAwaiter<TController> GetAwaiter() => this;

#endif

		#endregion

		#region IPresentAwaiter

#if !NET35

		bool IPresentAwaiter<TController>.IsCompleted => ((IPresentAwaiter)this).IsCompleted;

		TController IPresentAwaiter<TController>.GetResult() => (TController)((IPresentAwaiter)this).GetResult();

#endif

		#endregion
	}
}
