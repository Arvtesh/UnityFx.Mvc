// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace UnityFx.App
{
	/// <summary>
	/// 
	/// </summary>
	internal class AppStateStackOperation : TaskCompletionSource<IAppState>
	{
		#region interface

		public IAppStateTransition Transition { get; }

		public CancellationToken CancellationToken { get; }

		public AppStateStackOperation(IAppStateTransition transition, CancellationToken ct)
		{
			Transition = transition;
			CancellationToken = ct;
		}

		#endregion
	}
}
