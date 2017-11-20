// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace UnityFx.App
{
	/// <summary>
	/// Implementation of <see cref="IAppStateStack"/>.
	/// </summary>
	internal sealed class AppStateStack<TState> : List<TState> where TState : class, IAppState
	{
		#region data
		#endregion

		#region interface
		#endregion
	}
}
