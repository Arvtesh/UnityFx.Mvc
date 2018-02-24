// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	internal class TestController_EventErrors : IAppStateEvents, IDisposable
	{
		private readonly ControllerMethodId _errorId;

		public TestController_EventErrors(IAppStateContext context)
		{
			_errorId = (ControllerMethodId)context.Args;

			if (_errorId == ControllerMethodId.Ctor)
			{
				throw new Exception();
			}
		}

		public IAsyncOperation OnPush()
		{
			if (_errorId == ControllerMethodId.OnPush)
			{
				throw new Exception();
			}

			return null;
		}

		public Task OnLoadContent(CancellationToken cancellationToken)
		{
			if (_errorId == ControllerMethodId.OnLoadContent)
			{
				throw new Exception();
			}

			return Task.CompletedTask;
		}

		public void OnActivate(bool firstTime)
		{
			if (_errorId == ControllerMethodId.OnActivate)
			{
				throw new Exception();
			}
		}

		public void OnDeactivate()
		{
			if (_errorId == ControllerMethodId.OnDectivate)
			{
				throw new Exception();
			}
		}

		public void OnPop()
		{
			if (_errorId == ControllerMethodId.OnPop)
			{
				throw new Exception();
			}
		}

		public void Dispose()
		{
			if (_errorId == ControllerMethodId.Dispose)
			{
				throw new Exception();
			}
		}
	}
}
