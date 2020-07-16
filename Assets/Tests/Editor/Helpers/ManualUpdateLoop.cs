// Copyright (C) 2019 Alexander Bogarsukov. All rights reserved.
// See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFx.Mvc
{
	public class ManualUpdateLoop : IPresenterEventSource
	{
		private List<IPlayerLoopEvents> _presenters = new List<IPlayerLoopEvents>();

		public void Update()
		{
			foreach (var presenter in _presenters)
			{
				presenter.OnUpdate();
			}
		}

		public void AddPresenter(IPlayerLoopEvents presenter)
		{
			_presenters.Add(presenter);
		}

		public void RemovePresenter(IPlayerLoopEvents presenter)
		{
			_presenters.Remove(presenter);
		}
	}
}
