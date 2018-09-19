// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Defines context data for <see cref="IPresentMiddleware"/>.
	/// </summary>
	/// <seealso cref="IPresentMiddleware"/>
	public interface IPresentContext
	{
		/// <summary>
		/// Gets the controller creation arguments.
		/// </summary>
		PresentArgs PresentArgs { get; }

		/// <summary>
		/// Gets a state that presented this one (or <see langword="null"/>).
		/// </summary>
		IAppState PresenterState { get; }

		/// <summary>
		/// Gets parent state.
		/// </summary>
		IAppState ParentState { get; }

		/// <summary>
		/// Gets parent controller (or <see langword="null"/>).
		/// </summary>
		IViewController ParentController { get; }

		/// <summary>
		/// Gets a <see cref="IServiceProvider"/> that can be used to resolve controller dependencies.
		/// </summary>
		IServiceProvider ServiceProvider { get; }
	}
}
