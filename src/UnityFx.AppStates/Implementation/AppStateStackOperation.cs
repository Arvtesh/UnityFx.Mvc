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

		public IAppState OwnerState { get; }

		public Type StateType { get; }

		public object StateArgs { get; }

		public AppStateStackOperation(PushOptions options, IAppState owner, Type stateType, object stateArgs)
		{
			Operation = StackOperation.Push;
			Options = options;
			OwnerState = owner;
			StateType = stateType;
			StateArgs = stateArgs;
		}

		public AppStateStackOperation(IAppState owner)
		{
			Operation = StackOperation.Pop;
			OwnerState = owner;
		}

		#endregion
	}
}
