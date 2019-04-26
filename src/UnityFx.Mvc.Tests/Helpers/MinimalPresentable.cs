// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

namespace UnityFx.Mvc
{
	public class MinimalPresentable : IPresentable
	{
		private IView _view = new FakeView();

		public bool IsViewLoaded => _view != null;
		public IView View => _view;
		public bool IsDismissed => _view == null;

		public event EventHandler<AsyncCompletedEventArgs> LoadViewCompleted;
		public event EventHandler Dismissed;

		public MinimalPresentable()
		{
		}

		public void Dismiss()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (_view != null)
			{
				UnloadView();
				Dismissed?.Invoke(this, EventArgs.Empty);
			}
		}

		public bool InvokeCommand(string commandName, object args)
		{
			return false;
		}

		public void LoadViewAsync()
		{
			if (_view == null)
			{
				throw new InvalidOperationException();
			}
			else
			{
				LoadViewCompleted?.Invoke(this, new AsyncCompletedEventArgs(null, false, null));
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
