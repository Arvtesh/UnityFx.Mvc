// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	public class ViewController_ErrorsAsync : ViewController_Errors
	{
		public ViewController_ErrorsAsync(IViewControllerContext ctx)
			: base(ctx)
		{
		}

		protected override IAsyncOperation GetAsync()
		{
			return AsyncResult.Delay(10);
		}
	}
}
