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
		public Task<GameObject> LoadViewPrefabAsync(string resourceId)
		{
			return Task.FromResult<GameObject>(null);
		}

		public Task<IView> PresentViewAsync(string prefabPath, int layer, int zIndex, ViewControllerFlags flags, Transform parent)
		{
			switch (prefabPath)
			{
				case "MessageBox":
					return Task.FromResult<IView>(new MessageBoxView());
			}

			return Task.FromResult<IView>(new DefaultView());
		}
	}
}
