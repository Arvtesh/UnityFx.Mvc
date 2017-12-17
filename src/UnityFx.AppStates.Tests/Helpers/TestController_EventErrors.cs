// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.App.Tests
{
	internal class TestController_EventErrors : IAppStateController, IAppStateEvents
	{
		public void OnActivate(bool firstTime)
		{
			throw new Exception();
		}

		public void OnDeactivate()
		{
			throw new Exception();
		}

		public void OnPop()
		{
			throw new Exception();
		}

		public void OnPush()
		{
			throw new Exception();
		}
	}
}
