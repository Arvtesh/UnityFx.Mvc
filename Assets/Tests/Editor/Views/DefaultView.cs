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
		public event EventHandler<CommandEventArgs> Command;

		public Transform Transform => null;
		public bool Enabled { get; set; } = true;

		public void OnCommand()
		{
			Command?.Invoke(this, new CommandEventArgs());
		}

		public void Dispose()
		{
		}
	}
}
