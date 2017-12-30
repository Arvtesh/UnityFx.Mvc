// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Implementatino of <see cref="IAppStateViewFactory"/>.
	/// </summary>
	internal class AppStateViewFactory : MonoBehaviour, IAppStateViewFactory
	{
		#region data
		#endregion

		#region interface

		public void Initialize()
		{
		}

		#endregion

		#region MonoBehaviour
		#endregion

		#region IAppStateViewFactory

		public IAppStateView CreateView(string name, IAppStateView insertAfter)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region implementation
		#endregion
	}
}
