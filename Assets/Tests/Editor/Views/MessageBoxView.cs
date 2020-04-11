// Copyright (C) 2019 Alexander Bogarsukov. All rights reserved.
// See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Mvc
{
	public class MessageBoxView : IView, IConfigurable<Extensions.MessageBoxArgs>
	{
		private bool _disposed;

		public event EventHandler Disposed;
		public event EventHandler<CommandEventArgs> Command;

		public string ResourceId => null;
		public Transform Transform => null;
		public bool Enabled { get; set; } = true;

		public void Configure(Extensions.MessageBoxArgs args)
		{
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				Disposed?.Invoke(this, EventArgs.Empty);
			}
		}

		protected void OnCommand()
		{
			Command?.Invoke(this, new CommandEventArgs());
		}
	}
}
