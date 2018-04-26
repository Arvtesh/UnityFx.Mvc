// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	internal class TestController_ConstructorWithMultipleArguments : AppStateController
	{
		public IServiceProvider ServiceProvider { get; }

		public object Obj { get; }

		public TestController_ConstructorWithMultipleArguments(IServiceProvider sp, AppState s, object o)
			: base(s)
		{
			ServiceProvider = sp;
			Obj = o;
		}
	}
}
