// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A transition animation that is played during states switching.
	/// </summary>
	public interface IAppStateTransition
	{
		/// <summary>
		/// Plays a 'push' transition.
		/// </summary>
		Task PlayPushTransition(IAppState toState, CancellationToken cancellationToken);

		/// <summary>
		/// Plays a 'push' transition.
		/// </summary>
		Task PlayPushTransition(IAppState fromState, IAppState toState, CancellationToken cancellationToken);

		/// <summary>
		/// Plays a 'set' transition.
		/// </summary>
		Task PlaySetTransition(IAppState fromState, IAppState toState, CancellationToken cancellationToken);

		/// <summary>
		/// Plays a 'pop' transition.
		/// </summary>
		Task PlayPopTransition(IAppState state, CancellationToken cancellationToken);
	}
}
