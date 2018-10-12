// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	public class ViewController_Minimal : IViewController
	{
		private readonly IViewControllerContext _ctx;

		public int Id { get; }
		public string Name { get; set; }

		public ViewController_Minimal(IViewControllerContext ctx)
		{
			_ctx = ctx;
		}

		public IAsyncOperation DismissAsync()
		{
			return _ctx.DismissAsync();
		}
	}
}
