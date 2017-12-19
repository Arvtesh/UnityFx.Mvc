// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnityFx.App
{
	/// <summary>
	/// 
	/// </summary>
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

		#region Object

		public override string ToString()
		{
			return "PopState";
		}

		#endregion
	}
}
