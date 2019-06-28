// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	public class MinimalPresentable : IViewController
	{
		private IView _view;
		private bool _disposed;

		public event EventHandler Disposed;
		public IView View => _view;

		public MinimalPresentable()
		{
		}

		public virtual void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				_view?.Dispose();

				Disposed?.Invoke(this, EventArgs.Empty);
			}
		}

		public virtual bool InvokeCommand(string commandName, object args)
		{
			return false;
		}
	}
}
