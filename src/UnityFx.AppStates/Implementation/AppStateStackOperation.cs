// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UnityFx.App
{
	/// <summary>
	/// A generic app state stack operation.
	/// </summary>
	internal class AppStateStackOperation : TaskCompletionSource<IAppState>
	{
		#region data

		private List<Exception> _exceptions;

		#endregion

		#region interface

		public IAppStateTransition Transition { get; }

		public CancellationToken CancellationToken { get; }

		public AppStateStackOperation(IAppStateTransition transition, CancellationToken ct)
		{
			Transition = transition;
			CancellationToken = ct;
		}

		public void AddException(Exception e)
		{
			if (_exceptions == null)
			{
				_exceptions = new List<Exception>() { e };
			}
			else
			{
				_exceptions.Add(e);
			}
		}

		public new void SetResult(IAppState result)
		{
			if (_exceptions != null)
			{
				if (_exceptions.Count > 1)
				{
					SetException(_exceptions);
				}
				else
				{
					SetException(_exceptions[0]);
				}
			}
			else if (CancellationToken.IsCancellationRequested)
			{
				SetCanceled();
			}
			else
			{
				base.SetResult(result);
			}
		}

		public new bool TrySetException(Exception e)
		{
			if (_exceptions != null)
			{
				_exceptions.Add(e);
				return TrySetException(_exceptions);
			}

			return base.TrySetException(e);
		}

		#endregion
	}
}
