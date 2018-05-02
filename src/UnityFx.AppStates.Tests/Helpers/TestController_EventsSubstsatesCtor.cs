// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UnityFx.AppStates
{
	internal class TestController_EventsSubstsatesCtor : TestController_Events
	{
		public TestController_EventsSubstsatesCtor(AppState state)
			: base(state)
		{
			//PushSubstateAsync(typeof(TestController_Events), state.CreationArgs);
		}
	}
}
