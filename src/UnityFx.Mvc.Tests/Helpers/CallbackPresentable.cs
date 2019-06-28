// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace UnityFx.Mvc
{
	public class CallbackPresentable : MinimalPresentable, IViewControllerEvents
	{
		public int OnActivateCounter { get; private set; }
		public int OnDeactivateCounter { get; private set; }

		public void OnActivate()
		{
			OnActivateCounter++;
		}

		public void OnDeactivate()
		{
			OnDeactivateCounter++;
		}
	}
}
