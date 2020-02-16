// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	public class ErrorEventsController : IViewController, IViewControllerEvents
	{
		private readonly int _errorId;

		public IView View { get; }

		public ErrorEventsController(IView view, PresentArgs<int> args)
		{
			View = view;
			_errorId = args.Value;

			if (_errorId == 0)
			{
				throw new InvalidOperationException();
			}
		}

		void IViewControllerEvents.OnPresent()
		{
			if (_errorId == 1)
			{
				throw new InvalidOperationException();
			}
		}

		void IViewControllerEvents.OnDismiss()
		{
			if (_errorId == 4)
			{
				throw new InvalidOperationException();
			}
		}

		void IViewControllerEvents.OnActivate()
		{
			if (_errorId == 2)
			{
				throw new InvalidOperationException();
			}
		}

		void IViewControllerEvents.OnDeactivate()
		{
			if (_errorId == 3)
			{
				throw new InvalidOperationException();
			}
		}
	}
}
