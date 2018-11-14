// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	internal class FakeServiceProvider : IServiceProvider
	{
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(IServiceProvider))
			{
				return this;
			}

			return null;
		}
	}
}
