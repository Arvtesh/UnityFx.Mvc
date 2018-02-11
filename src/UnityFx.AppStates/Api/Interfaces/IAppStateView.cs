// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic application view.
	/// </summary>
	/// <seealso cref="IAppStateViewFactory"/>
	public interface IAppStateView : IAppView
	{
		/// <summary>
		/// Returns <see langword="true"/> if the view should cover all screen (views under it are not visible);
		/// <see langword="false"/> otherwise.
		/// </summary>
		/// <seealso cref="SetExclusive(bool)"/>
		bool IsExclusive { get; }

		/// <summary>
		/// Sets exclusive mode for the view.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown if the view has been disposed.</exception>
		/// <seealso cref="IsExclusive"/>
		void SetExclusive(bool exclusive);
	}
}
