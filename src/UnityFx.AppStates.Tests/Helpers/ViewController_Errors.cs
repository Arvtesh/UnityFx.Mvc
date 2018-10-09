// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	internal class ViewController_Errors : IViewController, IPresentable, IPresentableEvents, IDisposable
	{
		private readonly IViewControllerContext _ctx;

		public string Id => "ErrorsViewController";

		public bool OnPresentThrows { get; set; }
		public bool OnDismissThrows { get; set; }
		public bool OnActivateThrows { get; set; }
		public bool OnDeactivateThrows { get; set; }
		public bool PresentThrows { get; set; }
		public bool DismissThrows { get; set; }
		public bool DisposeThrows { get; set; }

		public ViewController_Errors(IViewControllerContext ctx)
		{
			_ctx = ctx;
		}

		public IAsyncOperation DismissAsync()
		{
			return _ctx.DismissAsync();
		}

		public void Dispose()
		{
			if (DisposeThrows)
			{
				throw new Exception("Dispose");
			}
		}

		public IAsyncOperation PresentAsync(IPresentContext presentContext)
		{
			if (PresentThrows)
			{
				throw new Exception("PresentAsync");
			}

			return GetAsync();
		}

		public IAsyncOperation DismissAsync(IDismissContext dismissContext)
		{
			if (DismissThrows)
			{
				throw new Exception("DismissAsync");
			}

			return GetAsync();
		}

		public void OnPresent()
		{
			if (OnPresentThrows)
			{
				throw new Exception("OnPresent");
			}
		}

		public void OnActivate()
		{
			if (OnActivateThrows)
			{
				throw new Exception("OnActivate");
			}
		}

		public void OnDeactivate()
		{
			if (OnDeactivateThrows)
			{
				throw new Exception("OnDeactivate");
			}
		}

		public void OnDismiss()
		{
			if (OnDismissThrows)
			{
				throw new Exception("OnDismiss");
			}
		}

		protected virtual IAsyncOperation GetAsync()
		{
			return null;
		}
	}
}
