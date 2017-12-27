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

		public IEnumerable<Exception> Exceptions => _exceptions;

		public bool HasExceptions => _exceptions != null;

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

		#endregion
	}
}
