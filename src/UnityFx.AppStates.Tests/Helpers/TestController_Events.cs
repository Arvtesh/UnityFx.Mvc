// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace UnityFx.App.Tests
{
	internal class TestController_Events : IAppStateController, IAppStateEvents, IAppStateContent, IDisposable
	{
		private int _counter = 0;

		public int OnActivateIndex { get; private set; }
		public int OnActivateThreadId { get; private set; }
		public int OnDeactivateIndex { get; private set; }
		public int OnDeactivateThreadId { get; private set; }
		public int OnPushIndex { get; private set; }
		public int OnPushThreadId { get; private set; }
		public int OnPopIndex { get; private set; }
		public int OnPopThreadId { get; private set; }
		public int LoadContentIndex { get; private set; }
		public int LoadContentThreadId { get; private set; }
		public int DisposeIndex { get; private set; }
		public int DisposeThreadId { get; private set; }

		public Task LoadContent(CancellationToken cancellationToken)
		{
			LoadContentIndex = ++_counter;
			LoadContentThreadId = Thread.CurrentThread.ManagedThreadId;
			return Task.Delay(100);
		}

		public void OnActivate(bool firstTime)
		{
			OnActivateIndex = ++_counter;
			OnActivateThreadId = Thread.CurrentThread.ManagedThreadId;
		}

		public void OnDeactivate()
		{
			OnDeactivateIndex = ++_counter;
			OnDeactivateThreadId = Thread.CurrentThread.ManagedThreadId;
		}

		public void OnPop()
		{
			OnPopIndex = ++_counter;
			OnPopThreadId = Thread.CurrentThread.ManagedThreadId;
		}

		public void OnPush()
		{
			OnPushIndex = ++_counter;
			OnPushThreadId = Thread.CurrentThread.ManagedThreadId;
		}

		public void Dispose()
		{
			DisposeIndex = ++_counter;
			DisposeThreadId = Thread.CurrentThread.ManagedThreadId;
		}
	}
}
