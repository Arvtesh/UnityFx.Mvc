// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.App
{
	/// <summary>
	/// 
	/// </summary>
	internal class AppStateStackOperation : TaskCompletionSource<IAppState>
	{
		#region interface

		public StackOperation Operation { get; }

		public PushOptions Options { get; }

		public IAppStateTransition Transition { get; }

		public IAppStateInternal State { get; }

		public Type StateType { get; }

		public object StateArgs { get; }

		public AppStateStackOperation(PushOptions options, IAppStateInternal owner, Type stateType, object stateArgs)
		{
			Operation = StackOperation.Push;
			Options = options;
			State = owner;
			StateType = stateType;
			StateArgs = stateArgs;
		}

		public AppStateStackOperation(IAppStateInternal state)
		{
			Operation = StackOperation.Pop;
			State = state;
		}

		#endregion
	}
}
