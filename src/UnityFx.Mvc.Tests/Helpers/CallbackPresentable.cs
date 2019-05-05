// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace UnityFx.Mvc
{
	public class CallbackPresentable : MinimalPresentable, IPresentableEvents
	{
		public int OnActivateCounter { get; private set; }
		public int OnDeactivateCounter { get; private set; }
		public int LoadViewAsyncCounter { get; private set; }

		public override void LoadViewAsync()
		{
			LoadViewAsyncCounter++;
			base.LoadViewAsync();
		}

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
