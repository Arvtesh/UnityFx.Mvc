// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Arguments for state manager error events.
	/// </summary>
	public class AppStateOperationEventArgs : AppStateEventArgs
	{
		/// <summary>
		/// Returns the operation result. Read only.
		/// </summary>
		public IAppStateOperation Operation { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="AppStateOperationEventArgs"/> class.
		/// </summary>
		public AppStateOperationEventArgs(IAppStateOperation op, IAppState state)
			: base(state)
		{
			Operation = op;
		}
	}
}