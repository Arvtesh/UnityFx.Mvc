// Copyright (C) 2019 Alexander Bogarsukov. All rights reserved.
// See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	public class DefaultViewFactory : IViewFactory
	{
		public async Task<IView> CreateViewAsync(Type controllerType, int zIndex, PresentOptions options, Transform parent)
		{
			await Task.Delay(10);
			return new DefaultView();
		}
	}
}
