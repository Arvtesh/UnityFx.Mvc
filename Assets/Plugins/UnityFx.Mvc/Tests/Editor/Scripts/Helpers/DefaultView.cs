// Copyright (C) 2019 Alexander Bogarsukov. All rights reserved.
// See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	public class DefaultView : IView
	{
		private bool _disposed;

		public event EventHandler Disposed;
		public event EventHandler<CommandEventArgs> Command;

		public Transform Transform => null;
		public bool Enabled { get; set; } = true;
		public ISite Site { get; set; }

		public void OnCommand()
		{
			Command?.Invoke(this, new CommandEventArgs("Dummy"));
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				Disposed.Invoke(this, EventArgs.Empty);
			}
		}
	}
}
