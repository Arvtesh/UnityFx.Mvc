// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

namespace UnityFx.Mvc
{
	internal class FakeView : IView
	{
		private bool _disposed;

		public event EventHandler<CommandEventArgs> Command;
		public event EventHandler Disposed;

		public string Name { get; set; }
		public bool Enabled { get; set; }
		public ISite Site { get; set; }

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				Disposed?.Invoke(this, EventArgs.Empty);
			}
		}
	}
}
