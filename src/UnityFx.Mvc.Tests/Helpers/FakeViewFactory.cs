// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace UnityFx.Mvc
{
	internal class FakeViewFactory : IViewFactory
	{
		public async Task<IView> CreateViewAsync(Type controllerType, int zIndex)
		{
			await Task.Yield();
			return new FakeView();
		}
	}
}
