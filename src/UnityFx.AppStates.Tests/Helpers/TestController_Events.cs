// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	internal class TestController_Events : AppStateController
	{
		private readonly ICollection<MethodCallInfo> _calls;

		public TestController_Events(IAppStateContext context)
			: base(context)
		{
			_calls = context.CreationArgs.Data as ICollection<MethodCallInfo>;
			_calls.Add(new MethodCallInfo(this, ControllerMethodId.Ctor));
		}

		protected override AsyncResult OnLoadContent()
		{
			_calls.Add(new MethodCallInfo(this, ControllerMethodId.OnPush));
			_calls.Add(new MethodCallInfo(this, ControllerMethodId.OnLoadContent));
			return base.OnLoadContent();
		}

		protected override void OnActivate(bool firstActivated)
		{
			base.OnActivate(firstActivated);
			_calls.Add(new MethodCallInfo(this, ControllerMethodId.OnActivate));
		}

		protected override void OnDeactivate()
		{
			_calls.Add(new MethodCallInfo(this, ControllerMethodId.OnDectivate));
			base.OnDeactivate();
		}

		protected override void Dispose(bool disposing)
		{
			_calls.Add(new MethodCallInfo(this, ControllerMethodId.OnPop));
			_calls.Add(new MethodCallInfo(this, ControllerMethodId.Dispose));
			base.Dispose(disposing);
		}
	}
}
