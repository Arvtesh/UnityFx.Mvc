// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.Mvc
{
	public class ViewController_MinimalAsync : IViewController, IAsyncPresentable
	{
		private readonly IViewControllerContext _ctx;

		public string Name { get; set; }
		public bool IsViewLoaded { get; }
		public IView View { get; }

		public ViewController_MinimalAsync(IViewControllerContext ctx)
		{
			_ctx = ctx;
		}

		public bool InvokeCommand(string commandName, object args)
		{
			return false;
		}

		public void Dispose()
		{
			_ctx.DismissAsync();
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
