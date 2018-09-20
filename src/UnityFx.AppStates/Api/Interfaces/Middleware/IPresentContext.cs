// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
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
		/// Gets operation-specific data.
		/// </summary>
		IDictionary<string, object> Properties { get; }

		/// <summary>
		/// Gets a state that presented <see cref="NextState"/> or a state being dismissed (for dismiss transitions).
		/// </summary>
		IAppState PrevState { get; }

		/// <summary>
		/// Gets parent controller (or <see langword="null"/>).
		/// </summary>
		IViewController PrevController { get; }

		/// <summary>
		/// Gets the new (target) state. For dismiss transitions it is always <see langword="null"/>.
		/// </summary>
		IAppState NextState { get; }

		/// <summary>
		/// Gets parent controller (or <see langword="null"/>).
		/// </summary>
		IViewController NextController { get; }

		/// <summary>
		/// Gets a <see cref="IServiceProvider"/> that can be used to resolve controller dependencies.
		/// </summary>
		IServiceProvider ServiceProvider { get; }
	}
}
