// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityFx.AppStates
{
	internal class AppStateServiceSettings : IAppStateServiceSettings
	{
		#region data

		private readonly TraceSource _traceSource;

		#endregion

		#region interface

		public AppStateServiceSettings(TraceSource traceSource)
		{
			_traceSource = traceSource;
		}

		#endregion

		#region IAppStateServiceSettings

		public SourceSwitch TraceSwitch { get => _traceSource.Switch; set => _traceSource.Switch = value; }

		public TraceListenerCollection TraceListeners => _traceSource.Listeners;

		public int MaxNumberOfPendingOperations
		{
			get
			{
				return 0;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}
