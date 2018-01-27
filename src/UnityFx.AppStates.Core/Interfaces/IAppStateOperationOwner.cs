// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Store operation owner.
	/// </summary>
	internal interface IAppStateOperationOwner
	{
		/// <summary>
		/// Returns the parent store.
		/// </summary>
		TraceSource TraceSource { get; }
	}
}
