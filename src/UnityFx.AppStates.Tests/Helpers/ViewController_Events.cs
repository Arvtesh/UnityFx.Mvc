// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	public class ViewController_Events : IViewController, IPresentable, IPresentableEvents, IDisposable
	{
		private readonly IViewControllerContext _ctx;
		private int _index;

		public string Name { get; set; }
		public IView View { get; }

		public int OnPresentIndex { get; private set; }
		public int OnDismissIndex { get; private set; }
		public int OnActivateIndex { get; private set; }
		public int OnDeactivateIndex { get; private set; }
		public int PresentIndex { get; private set; }
		public int DismissIndex { get; private set; }
		public int DisposeIndex { get; private set; }

		public ViewController_Events(IViewControllerContext ctx)
		{
			_ctx = ctx;
		}

		public IAsyncOperation DismissAsync()
		{
			return _ctx.DismissAsync();
		}

		public void Dispose()
		{
			DisposeIndex = ++_index;
		}

		public IAsyncOperation PresentAsync(IPresentContext presentContext)
		{
			PresentIndex = ++_index;
			return GetAsync();
		}

		public IAsyncOperation DismissAsync(IDismissContext dismissContext)
		{
			DismissIndex = ++_index;
			return GetAsync();
		}

		public void OnPresent()
		{
			OnPresentIndex = ++_index;
		}

		public void OnActivate()
		{
			OnActivateIndex = ++_index;
		}

		public void OnDeactivate()
		{
			OnDeactivateIndex = ++_index;
		}

		public void OnDismiss()
		{
			OnDismissIndex = ++_index;
		}

		protected virtual IAsyncOperation GetAsync()
		{
			return null;
		}
	}
}
