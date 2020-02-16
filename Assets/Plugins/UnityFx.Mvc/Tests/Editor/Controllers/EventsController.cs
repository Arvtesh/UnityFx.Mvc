// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	public class EventsController : IViewController, IViewControllerEvents
	{
		private int _callId;

		public int PresentCallId { get; private set; }
		public int ActivateCallId { get; private set; }
		public int DeactivateCallId { get; private set; }
		public int DismissCallId { get; private set; }

		public IView View { get; }

		public EventsController(IView view)
		{
			View = view;
		}

		void IViewControllerEvents.OnPresent()
		{
			PresentCallId = ++_callId;
		}

		void IViewControllerEvents.OnDismiss()
		{
			DismissCallId = ++_callId;
		}

		void IViewControllerEvents.OnActivate()
		{
			ActivateCallId = ++_callId;
		}

		void IViewControllerEvents.OnDeactivate()
		{
			DeactivateCallId = ++_callId;
		}
	}
}
