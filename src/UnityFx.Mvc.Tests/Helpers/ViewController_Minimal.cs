// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.Mvc
{
	public class ViewController_Minimal : IViewController
	{
		private readonly IPresentContext _ctx;

		public string Name { get; set; }
		public bool IsViewLoaded { get; }
		public IView View { get; }

		public ViewController_Minimal(IPresentContext ctx)
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
	}
}
