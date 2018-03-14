// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;

namespace UnityFx.AppStates
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
		None = 0
	}

	/// <summary>
	/// A generic application state.
	/// </summary>
	/// <remarks>
	/// By design an application flow is a sequence of state switches. A state may represent a single screen,
	/// a dialog, a menu or some process without any visual representation. States are supposed to be as independent
	/// as possible. Only one state may be active (i.e. process user input) at time, but unlimited number of states may
	/// exist (be rendered on the screen and execute their code) at the same time.
	/// </remarks>
	/// <seealso href="http://gameprogrammingpatterns.com/state.html"/>
	/// <seealso href="https://en.wikipedia.org/wiki/State_pattern"/>
	public interface IAppState
	{
		/// <summary>
		/// Gets unique static state type identifier.
		/// </summary>
		string Id { get; }

		/// <summary>
		/// Gets the state flags.
		/// </summary>
		AppStateFlags Flags { get; }

		/// <summary>
		/// Gets a path to this state.
		/// </summary>
		string Path { get; }

		/// <summary>
		/// Gets the arguments used to create the state.
		/// </summary>
		PushStateArgs CreationArgs { get; }

		/// <summary>
		/// Gets a deeplink that was used to navigate to this state.
		/// </summary>
		Uri Deeplink { get; }

		/// <summary>
		/// Gets parent state (if any).
		/// </summary>
		IAppState ParentState { get; }

		/// <summary>
		/// Gets owner state (if any). Owner state is the state that pushed this one onto the stack.
		/// </summary>
		IAppState OwnerState { get; }

		/// <summary>
		/// Gets a view instance attached to the state.
		/// </summary>
		IAppStateView View { get; }

		/// <summary>
		/// Gets a user-defined view controller instance attached to the state.
		/// </summary>
		IAppStateController Controller { get; }

		/// <summary>
		/// Gets a value indicating whether this state is active (i.e. it is a top state and can processes user input).
		/// </summary>
		bool IsActive { get; }

		/// <summary>
		/// Gets a child states collection.
		/// </summary>
		IReadOnlyCollection<IAppState> ChildStates { get; }
	}
}
