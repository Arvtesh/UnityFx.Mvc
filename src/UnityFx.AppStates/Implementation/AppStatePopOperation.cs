// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnityFx.App
{
	internal class AppStatePopOperation : AppStateStackOperation
	{
		#region interface

		public AppState State { get; }

		public AppStatePopOperation(AppState state, IAppStateTransition t, CancellationToken ct)
			: base(t, ct)
		{
			State = state;
		}

		#endregion

		#region IAppStateOperationInfo

		public override AppStateOperationType Type
		{
			get
			{
				if (State != null)
				{
					return AppStateOperationType.Pop;
				}

				return AppStateOperationType.PopAll;
			}
		}

		public override object Args => null;

		public override IAppState Result => null;

		public override IAppState Target => State;

		#endregion

		#region Object

		public override string ToString()
		{
			if (State != null)
			{
				return "PopState " + State.Name;
			}

			return "PopAll";
		}

		#endregion
	}
}
