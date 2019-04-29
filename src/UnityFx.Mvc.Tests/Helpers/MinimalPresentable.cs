// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace UnityFx.Mvc
{
	public class MinimalPresentable : IPresentable
	{
		private IView _view;
		private bool _presented;
		private bool _dismissed;
		private bool _disposed;

		public bool IsViewLoaded => _view != null;
		public IView View => _view;
		public bool IsDismissed => _dismissed;
		public bool IsPresented => _presented;

		public event EventHandler<AsyncCompletedEventArgs> LoadViewCompleted;
		public event EventHandler<AsyncCompletedEventArgs> Presented;
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

		public async void LoadViewAsync()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			if (_dismissed)
			{
				throw new InvalidOperationException();
			}
			else if (_view == null)
			{
				await Task.Delay(10);
				_view = new FakeView();

				var args = new AsyncCompletedEventArgs(null, false, null);

				LoadViewCompleted?.Invoke(this, args);

				if (!_presented)
				{
					_presented = true;
					Presented?.Invoke(this, args);
				}
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
