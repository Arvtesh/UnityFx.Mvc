// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace UnityFx.App
{
	/// <summary>
	/// A transition animation that is played during states switching.
	/// </summary>
	public interface IAppStateTransition
	{
		/// <summary>
		/// 
		/// </summary>
		Task PlayPushTransition(IAppState toState);

		/// <summary>
		/// 
		/// </summary>
		Task PlayPushTransition(IAppState fromState, IAppState toState);

		/// <summary>
		/// 
		/// </summary>
		Task PlaySetTransition(IAppState fromState, IAppState toState);

		/// <summary>
		/// 
		/// </summary>
		Task PlayPopTransition(IAppState state);
	}
}
