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

		public StackOperation Operation { get; }

		public IAppStateTransition Transition { get; }

		public CancellationToken CancellationToken { get; }

		public AppStateStackOperation(StackOperation op, IAppStateTransition transition, CancellationToken ct)
		{
			Operation = op;
			Transition = transition;
			CancellationToken = ct;
		}

		#endregion
	}
}
