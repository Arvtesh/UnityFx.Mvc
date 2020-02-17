// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	public class ErrorEventsController : IViewController, IViewControllerEvents
	{
		private readonly ControllerEvents _event;

		public IView View { get; }

		public ErrorEventsController(IView view, PresentArgs<ControllerEvents> args)
		{
			View = view;
			_event = args.Value;

			if (_event == ControllerEvents.Ctor)
			{
				throw new InvalidOperationException();
			}
		}

		void IViewControllerEvents.OnPresent()
		{
			if (_event == ControllerEvents.Present)
			{
				throw new InvalidOperationException();
			}
		}

		void IViewControllerEvents.OnDismiss()
		{
			if (_event == ControllerEvents.Dismiss)
			{
				throw new InvalidOperationException();
			}
		}

		void IViewControllerEvents.OnActivate()
		{
			if (_event == ControllerEvents.Activate)
			{
				throw new InvalidOperationException();
			}
		}

		void IViewControllerEvents.OnDeactivate()
		{
			if (_event == ControllerEvents.Deactivate)
			{
				throw new InvalidOperationException();
			}
		}
	}
}
