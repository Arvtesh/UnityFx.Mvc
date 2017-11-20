// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

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
		/// </summary>
		Popup = 1
	}

	/// <summary>
	/// Represents <see cref="IAppState"/>-related stuff. This is shared between <see cref="IAppState"/> and <see cref="IAppStateContext"/>.
	/// </summary>
	/// <seealso cref="IAppState"/>
	/// <seealso cref="IAppStateContext"/>
	public interface IAppStateInfo
	{
		/// <summary>
		/// Returns the <see cref="GameObject"/> this state is attached to. Read only.
		/// </summary>
		GameObject Go { get; }

		/// <summary>
		/// Returns state bounds (in world space) based on its content. Read only.
		/// </summary>
		Bounds Bounds { get; }

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
		/// Returns a view instance attached to the state. Read only.
		/// </summary>
		IAppView View { get; }
	}
}
