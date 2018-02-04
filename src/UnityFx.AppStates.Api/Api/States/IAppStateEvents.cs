// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;

namespace UnityFx.AppStates
{
	/// <summary>
	/// An <see cref="IAppState"/> extension. Implement it to receive notifications on the state lifetime events.
	/// </summary>
	/// <seealso cref="IAppState"/>
	public interface IAppStateEvents
	{
		/// <summary>
		/// Called right after the state has been pushed onto the stack.
		/// </summary>
		void OnPush();

		/// <summary>
		/// Called when the state is about to be removed from the stack.
		/// </summary>
		void OnPop();

		/// <summary>
		/// Called when the state has become active.
		/// </summary>
		void OnActivate(bool firstTime);

		/// <summary>
		/// Called when the state is about to become inactive.
		/// </summary>
		void OnDeactivate();

		/// <summary>
		/// Asyncronously loads state content. The state is not activated until this operation is finished.
		/// </summary>
		Task OnLoadContent(CancellationToken cancellationToken);
	}
}
