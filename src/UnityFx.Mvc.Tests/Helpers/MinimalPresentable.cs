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
		private bool _dismissed;
		private bool _disposed;

		public bool IsViewLoaded => _view != null;
		public IView View => _view;
		public bool IsDismissed => _dismissed;

		public event EventHandler<AsyncCompletedEventArgs> LoadViewCompleted;
		public event EventHandler Dismissed;

		public MinimalPresentable()
		{
		}

		public virtual void Dismiss()
		{
			if (!_dismissed)
			{
				_dismissed = true;
				Dismissed?.Invoke(this, EventArgs.Empty);
				Dispose();
			}
			
		}

		public virtual void Dispose()
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
			}
		}

		public virtual bool InvokeCommand(string commandName, object args)
		{
			return false;
		}

		public virtual async void LoadViewAsync()
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
				LoadViewCompleted?.Invoke(this, new AsyncCompletedEventArgs(null, false, null));
			}
		}

		public virtual void UnloadView()
		{
			if (_view != null)
			{
				_view.Dispose();
				_view = null;
			}
		}
	}
}
