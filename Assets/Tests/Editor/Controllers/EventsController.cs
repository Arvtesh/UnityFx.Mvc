// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace UnityFx.Mvc
{
	public class EventsController : IViewController, IPresentEvents
	{
		private readonly IPresentContext _context;
		private readonly ControllerEvents _throwOnEvent;
		private int _callId;

		public int PresentCallId { get; private set; }
		public int ActivateCallId { get; private set; }
		public int DeactivateCallId { get; private set; }
		public int DismissCallId { get; private set; }

		public IView View => _context.View;

		public EventsController(IPresentContext context)
		{
			_context = context;

			if (_throwOnEvent == ControllerEvents.Ctor)
			{
				throw new InvalidOperationException();
			}
		}

		void IPresentEvents.OnPresent()
		{
			PresentCallId = ++_callId;

			if (_throwOnEvent == ControllerEvents.Present)
			{
				throw new InvalidOperationException();
			}
		}

		void IPresentEvents.OnDismiss()
		{
			DismissCallId = ++_callId;

			if (_throwOnEvent == ControllerEvents.Dismiss)
			{
				throw new InvalidOperationException();
			}
		}

		void IPresentEvents.OnActivate()
		{
			ActivateCallId = ++_callId;

			if (_throwOnEvent == ControllerEvents.Activate)
			{
				throw new InvalidOperationException();
			}
		}

		void IPresentEvents.OnDeactivate()
		{
			DeactivateCallId = ++_callId;

			if (_throwOnEvent == ControllerEvents.Deactivate)
			{
				throw new InvalidOperationException();
			}
		}
	}
}
