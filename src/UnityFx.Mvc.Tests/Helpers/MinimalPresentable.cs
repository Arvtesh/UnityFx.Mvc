// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

namespace UnityFx.Mvc
{
	public class MinimalPresentable : IPresentable
	{
		private IView _view = new FakeView();

		private bool _presented;
		private bool _dismissed;
		private bool _disposed;

		public bool IsViewLoaded => _view != null;
		public IView View => _view;
		public bool IsDismissed => _presented;
		public bool IsPresented => _dismissed;

		public event EventHandler<AsyncCompletedEventArgs> LoadViewCompleted;
		public event EventHandler Presented;
		public event EventHandler Dismissed;
		public event EventHandler Disposed;

		public MinimalPresentable()
		{
		}

		public void Dismiss()
		{
			if (!_dismissed)
			{
				_dismissed = true;
				Dismissed?.Invoke(this, EventArgs.Empty);
				Dispose();
			}
			
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				if (!_dismissed)
				{
					_dismissed = true;
					Dismissed?.Invoke(this, EventArgs.Empty);
				}

				_disposed = true;
				UnloadView();
				Disposed?.Invoke(this, EventArgs.Empty);
			}
		}

		public bool InvokeCommand(string commandName, object args)
		{
			return false;
		}

		public void LoadViewAsync()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			if (_view == null || _dismissed)
			{
				throw new InvalidOperationException();
			}
			else
			{
				LoadViewCompleted?.Invoke(this, new AsyncCompletedEventArgs(null, false, null));
				_presented = true;
				Presented?.Invoke(this, EventArgs.Empty);
			}
		}

		public void UnloadView()
		{
			if (_view != null)
			{
				_view.Dispose();
				_view = null;
			}
		}
	}
}
