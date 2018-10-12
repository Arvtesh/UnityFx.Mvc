// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	public class ViewController_MinimalAsync : IViewController, IPresentable
	{
		private readonly IViewControllerContext _ctx;

		public int Id { get; }
		public string Name { get; set; }

		public ViewController_MinimalAsync(IViewControllerContext ctx)
		{
			_ctx = ctx;
		}

		public IAsyncOperation DismissAsync()
		{
			return _ctx.DismissAsync();
		}

		public IAsyncOperation PresentAsync(IPresentContext presentContext)
		{
			return AsyncResult.Delay(10);
		}

		public IAsyncOperation DismissAsync(IDismissContext dismissContext)
		{
			return AsyncResult.Delay(10);
		}
	}
}
