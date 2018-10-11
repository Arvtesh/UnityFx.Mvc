// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic application state.
	/// </summary>
	/// <remarks>
	/// By design an application flow is a sequence of state switches. A state may represent a single screen,
	/// a dialog or a menu. States are supposed to be as independent as possible. Only one state may be active
	/// (i.e. process user input) at time, but unlimited number of states may exist at the same time. Any
	/// state can be targeted by a deeplink.
	/// </remarks>
	/// <seealso href="http://gameprogrammingpatterns.com/state.html"/>
	/// <seealso href="https://en.wikipedia.org/wiki/State_pattern"/>
	public interface IAppState : IObjectId, ITreeListNode<IAppState>, IPresenter, IDeeplinkable, IDismissable, IDisposable
	{
		/// <summary>
		/// Gets a value indicating whether the instance is active.
		/// </summary>
		bool IsActive { get; }

		/// <summary>
		/// Gets a view controller attached to the state.
		/// </summary>
		IViewController Controller { get; }
	}
}
