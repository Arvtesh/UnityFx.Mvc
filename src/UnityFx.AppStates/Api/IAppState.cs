// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFx.App
{
	/// <summary>
	/// A generic application state.
	/// </summary>
	/// <remarks>
	/// <para>By design an application flow is a sequence of state switches. A state may represent a single screen,
	/// a dialog, a menu or some process without any visual representation. States are supposed to be as independent
	/// as possible. Only one state may be active (i.e. process user input) at time, but unlimited number of states may
	/// exist (be rendered on the screen and execute their code) at the same time.</para>
	/// </remarks>
	/// <seealso href="http://gameprogrammingpatterns.com/state.html"/>
	/// <seealso href="https://en.wikipedia.org/wiki/State_pattern"/>
	/// <seealso cref="IAppStateController"/>
	public interface IAppState : IAppStateInfo, IDisposable
	{
		/// <summary>
		/// Returns child states enumerator. Read only.
		/// </summary>
		IEnumerable<IAppState> Children { get; }

		/// <summary>
		/// Returns a user-defined view controller instance attached to the state. Read only.
		/// </summary>
		IAppStateController Controller { get; }
	}
}