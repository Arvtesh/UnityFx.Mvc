// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	public class TimerController : IViewController
	{
		private readonly IPresentContext _context;

		public IView View => _context.View;

		public TimerController(IPresentContext context)
		{
			_context = context;
			_context.Schedule(OnTimer, 0.1f);
		}

		public bool InvokeCommand<TCommand>(TCommand command)
		{
			return false;
		}

		private void OnTimer(float time)
		{
			_context.Dismiss();
		}
	}
}
