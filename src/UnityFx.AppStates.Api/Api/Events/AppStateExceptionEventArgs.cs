// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Arguments for state manager error events.
	/// </summary>
	public class AppStateExceptionEventArgs : EventArgs
	{
		/// <summary>
		/// Returns exception. Read only.
		/// </summary>
		public Exception Exception { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="AppStateExceptionEventArgs"/> class.
		/// </summary>
		public AppStateExceptionEventArgs(Exception e)
		{
			Exception = e;
		}
	}
}