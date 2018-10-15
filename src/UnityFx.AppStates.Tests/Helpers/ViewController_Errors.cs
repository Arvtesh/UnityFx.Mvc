// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	public class ViewController_Errors : IViewController, IPresentable, IPresentableEvents, IDisposable
	{
		private readonly IViewControllerContext _ctx;
		private readonly MyPresentArgs _args;

		public enum ThrowSource
		{
			None,
			Ctor,
			OnPresent,
			OnDismiss,
			OnActivate,
			OnDeactivate,
			Present,
			Dismiss,
			Dispose
		}

		public class MyPresentArgs : PresentArgs
		{
			private readonly ThrowSource _source;

			public MyPresentArgs(ThrowSource source)
			{
				_source = source;
			}

			public bool CtorThrows => _source == ThrowSource.Ctor;
			public bool OnPresentThrows => _source == ThrowSource.OnPresent;
			public bool OnDismissThrows => _source == ThrowSource.OnDismiss;
			public bool OnActivateThrows => _source == ThrowSource.OnActivate;
			public bool OnDeactivateThrows => _source == ThrowSource.OnDeactivate;
			public bool PresentThrows => _source == ThrowSource.Present;
			public bool DismissThrows => _source == ThrowSource.Dismiss;
			public bool DisposeThrows => _source == ThrowSource.Dispose;
		}

		public string Name { get; set; }
		public IView View { get; }

		public ViewController_Errors(IViewControllerContext ctx)
		{
			_ctx = ctx;
			_args = (MyPresentArgs)ctx.PresentArgs;

			if (_args.CtorThrows)
			{
				throw new Exception("Ctor");
			}
		}

		public IAsyncOperation DismissAsync()
		{
			return _ctx.DismissAsync();
		}

		public void Dispose()
		{
			if (_args.DisposeThrows)
			{
				throw new Exception("Dispose");
			}
		}

		public IAsyncOperation PresentAsync(IPresentContext presentContext)
		{
			if (_args.PresentThrows)
			{
				throw new Exception("PresentAsync");
			}

			return GetAsync();
		}

		public IAsyncOperation DismissAsync(IDismissContext dismissContext)
		{
			if (_args.DismissThrows)
			{
				throw new Exception("DismissAsync");
			}

			return GetAsync();
		}

		public void OnPresent()
		{
			if (_args.OnPresentThrows)
			{
				throw new Exception("OnPresent");
			}
		}

		public void OnActivate()
		{
			if (_args.OnActivateThrows)
			{
				throw new Exception("OnActivate");
			}
		}

		public void OnDeactivate()
		{
			if (_args.OnDeactivateThrows)
			{
				throw new Exception("OnDeactivate");
			}
		}

		public void OnDismiss()
		{
			if (_args.OnDismissThrows)
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
