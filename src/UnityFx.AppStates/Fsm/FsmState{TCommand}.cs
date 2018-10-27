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
		#region data

		private readonly FsmService<TCommand> _fsm;

		#endregion

		#region interface

		/// <summary>
		/// Gets a value indicating whether the state is active.
		/// </summary>
		public bool IsActive => _fsm.ActiveState == this;

		/// <summary>
		/// Initializes a new instance of the <see cref="FsmState{TCommand}"/> class.
		/// </summary>
		/// <param name="fsm">Parent FSM.</param>
		protected FsmState(FsmService<TCommand> fsm)
		{
			_fsm = fsm;
		}

		/// <summary>
		/// TODO.
		/// </summary>
		/// <param name="stateType"></param>
		protected void SetState(Type stateType)
		{
			_fsm.SetState(stateType, null);
		}

		/// <summary>
		/// TODO.
		/// </summary>
		/// <param name="stateType"></param>
		/// <param name="args">User-supplied state arguments.</param>
		protected void SetState(Type stateType, object args)
		{
			_fsm.SetState(stateType, args);
		}

		/// <summary>
		/// TODO.
		/// </summary>
		protected void SetState<TState>() where TState : FsmState<TCommand>
		{
			_fsm.SetState(typeof(TState), null);
		}

		/// <summary>
		/// TODO.
		/// </summary>
		/// <param name="args">User-supplied state arguments.</param>
		protected void SetState<TState>(object args) where TState : FsmState<TCommand>
		{
			_fsm.SetState(typeof(TState), args);
		}

		/// <summary>
		/// Called by the parent FSM to process a command.
		/// </summary>
		/// <param name="cmd">The command to handle.</param>
		/// <seealso cref="OnEnter(object, object)"/>
		/// <seealso cref="OnExit(object)"/>
		protected internal virtual bool OnCommand(TCommand cmd)
		{
			return false;
		}

		/// <summary>
		/// Called by the parent FSM when the state is about to become active.
		/// </summary>
		/// <param name="prevState">The previously active state reference.</param>
		/// <param name="args">User-supplied state arguments.</param>
		/// <seealso cref="OnExit(object)"/>
		/// <seealso cref="OnCommand(TCommand)"/>
		protected internal virtual void OnEnter(object prevState, object args)
		{
		}

		/// <summary>
		/// Called by the parent FSM when the state is about to become inactive.
		/// </summary>
		/// <param name="nextState">The next active state reference.</param>
		/// <seealso cref="OnEnter(object, object)"/>
		/// <seealso cref="OnCommand(TCommand)"/>
		protected internal virtual void OnExit(object nextState)
		{
		}

		#endregion
	}
}
