// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace UnityFx.App.Tests
{
	internal class TestController_EventErrors : IAppStateController, IAppStateEvents, IAppStateContent, IDisposable
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

		public void OnPush()
		{
			if (_errorId == ControllerMethodId.OnPush)
			{
				throw new Exception();
			}
		}

		public Task LoadContent(CancellationToken cancellationToken)
		{
			if (_errorId == ControllerMethodId.LoadContent)
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
