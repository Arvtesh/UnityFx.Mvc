// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.Mvc
{
	public class ViewController_EventsAsync : ViewController_Events
	{
		public ViewController_EventsAsync(IViewControllerContext ctx)
			: base(ctx)
		{
		}

		protected override IAsyncOperation GetAsync()
		{
			return AsyncResult.Delay(10);
		}
	}
}
