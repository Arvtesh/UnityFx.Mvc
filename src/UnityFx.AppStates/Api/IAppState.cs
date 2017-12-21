// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnityFx.App
{
	/// <summary>
	/// Enumerates state-related flags.
	/// </summary>
	[Flags]
	public enum AppStateFlags
	{
		/// <summary>
		/// No flags.
		/// </summary>
		None = 0,

		/// <summary>
		/// If set, the view of a state under this one is rendered. Typically set for states that implement popup windows.
		/// Child states always have this flag set.
		/// </summary>
		Popup = 1
	}

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
	public interface IAppState
	{
		/// <summary>
		/// Returns the state name. Read only.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Returns the qualified state name. Read only.
		/// </summary>
		string FullName { get; }

		/// <summary>
		/// Returns state flags. Read only.
		/// </summary>
		AppStateFlags Flags { get; }

		/// <summary>
		/// Returns the state layer. Read only.
		/// </summary>
		int Layer { get; }

		/// <summary>
		/// Returns user-specified state arguments. Read only.
		/// </summary>
		object Args { get; }

		/// <summary>
		/// Returns a value indicating whether this state is active (i.e. it is a top state and can processes user input). Read only.
		/// </summary>
		bool IsActive { get; }

		/// <summary>
		/// Returns parent state (if any). Read only.
		/// </summary>
		IAppState Parent { get; }

		/// <summary>
		/// Returns the owner state (if any). Owner state is the state that pushed this one onto the stack. Read only.
		/// </summary>
		IAppState Owner { get; }

		/// <summary>
		/// Returns child states enumerator. Read only.
		/// </summary>
		IReadOnlyCollection<IAppState> ChildStates { get; }

		/// <summary>
		/// Returns a view instance attached to the state. Read only.
		/// </summary>
		IAppView View { get; }

		/// <summary>
		/// Returns a user-defined view controller instance attached to the state. Read only.
		/// </summary>
		IAppStateController Controller { get; }

		/// <summary>
		/// Pops the state from the state stack.
		/// </summary>
		Task CloseAsync();
	}
}