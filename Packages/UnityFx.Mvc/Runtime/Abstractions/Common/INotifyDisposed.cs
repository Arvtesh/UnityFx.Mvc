// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Notifies clients of dispose event.
	/// </summary>
	public interface INotifyDisposed
	{
		/// <summary>
		/// Raised when an instance is disposed.
		/// </summary>
		event EventHandler Disposed;
	}
}
