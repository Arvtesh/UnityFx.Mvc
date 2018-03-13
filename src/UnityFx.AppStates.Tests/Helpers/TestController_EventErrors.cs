// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	internal class TestController_EventErrors : AppStateController
	{
		private readonly ControllerMethodId _errorId;

		public TestController_EventErrors(IAppStateContext context)
			: base(context)
		{
			_errorId = (ControllerMethodId)context.CreationArgs.Data;

			if (_errorId == ControllerMethodId.Ctor)
			{
				throw new Exception();
			}

			if (_errorId == ControllerMethodId.OnPush)
			{
				throw new Exception();
			}
		}

		protected override void OnActivate(bool firstActivated)
		{
			base.OnActivate(firstActivated);

			if (_errorId == ControllerMethodId.OnActivate)
			{
				throw new Exception();
			}
		}

		protected override void OnDeactivate()
		{
			if (_errorId == ControllerMethodId.OnDectivate)
			{
				throw new Exception();
			}

			base.OnDeactivate();
		}

		protected override IAsyncOperation OnLoadContent()
		{
			if (_errorId == ControllerMethodId.OnLoadContent)
			{
				throw new Exception();
			}

			return base.OnLoadContent();
		}

		protected override void Dispose(bool disposing)
		{
			if (_errorId == ControllerMethodId.OnPop)
			{
				throw new Exception();
			}

			if (_errorId == ControllerMethodId.Dispose)
			{
				throw new Exception();
			}

			base.Dispose(disposing);
		}
	}
}
