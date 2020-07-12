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
		public Task<IView> CreateViewAsync(string prefabPath, int layer, int zIndex, ViewControllerFlags flags, Transform parent)
		{
			switch (prefabPath)
			{
				case "MessageBox":
					return Task.FromResult<IView>(new MessageBoxView());
			}

			return Task.FromResult<IView>(new DefaultView());
		}

		public void DestroyView(IView view)
		{
			view.Dispose();
		}
	}
}
