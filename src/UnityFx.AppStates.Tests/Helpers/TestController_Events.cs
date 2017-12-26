// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UnityFx.App.Tests
{
	internal class TestController_Events : IAppStateController, IAppStateEvents, IDisposable
	{
		private readonly ICollection<MethodCallInfo> _calls;

		public TestController_Events(IAppStateContext context)
		{
			_calls = context.Args as ICollection<MethodCallInfo>;
			_calls.Add(new MethodCallInfo(this, ControllerMethodId.Ctor));
		}

		public virtual void OnPush()
		{
			_calls.Add(new MethodCallInfo(this, ControllerMethodId.OnPush));
		}

		public virtual void OnPop()
		{
			_calls.Add(new MethodCallInfo(this, ControllerMethodId.OnPop));
		}

		public virtual void OnActivate(bool firstTime)
		{
			_calls.Add(new MethodCallInfo(this, ControllerMethodId.OnActivate));
		}

		public virtual void OnDeactivate()
		{
			_calls.Add(new MethodCallInfo(this, ControllerMethodId.OnDectivate));
		}

		public virtual Task OnLoadContent(CancellationToken cancellationToken)
		{
			_calls.Add(new MethodCallInfo(this, ControllerMethodId.OnLoadContent));
			return Task.Delay(1);
		}

		public virtual void Dispose()
		{
			_calls.Add(new MethodCallInfo(this, ControllerMethodId.Dispose));
		}
	}
}
