// Copyright (C) 2019 Alexander Bogarsukov. All rights reserved.
// See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFx.Mvc
{
	public class ManualUpdateLoop : IPresenterEventSource
	{
		private List<IPresenterEvents> _presenters = new List<IPresenterEvents>();

		public void Update()
		{
			foreach (var presenter in _presenters)
			{
				presenter.Update();
			}
		}

		public void AddPresenter(IPresenterEvents presenter)
		{
			_presenters.Add(presenter);
		}

		public void RemovePresenter(IPresenterEvents presenter)
		{
			_presenters.Remove(presenter);
		}
	}
}
