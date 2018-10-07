// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityFx.AppStates.Fsm
{
	/// <summary>
	/// A FSM state.
	/// </summary>
	/// <typeparam name="TCommand">Command type.</typeparam>
	/// <seealso href="https://en.wikipedia.org/wiki/Finite-state_machine"/>
	/// <seealso cref="FsmService{TCommand}"/>
	public abstract class FsmState<TCommand>
	{
		/// <summary>
		/// TODO.
		/// </summary>
		/// <param name="cmd"></param>
		protected abstract void OnCommand(TCommand cmd);

		/// <summary>
		/// TODO.
		/// </summary>
		protected abstract void OnEnter(object args);

		/// <summary>
		/// TODO.
		/// </summary>
		protected abstract void OnExit();
	}
}
