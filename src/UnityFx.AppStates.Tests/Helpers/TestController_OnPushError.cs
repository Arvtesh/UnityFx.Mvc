// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.App.Tests
{
	internal class TestController_OnPushError : IAppStateController, IAppStateEvents
	{
		public void OnActivate(bool firstTime)
		{
		}

		public void OnDeactivate()
		{
		}

		public void OnPop()
		{
		}

		public void OnPush()
		{
			throw new Exception();
		}
	}
}
