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
		#endregion

		#region interface
		#endregion

		#region IAppStateServiceSettings

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
