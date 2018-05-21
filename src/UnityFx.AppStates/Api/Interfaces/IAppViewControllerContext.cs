// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// tt
	/// </summary>
	public interface IAppViewControllerContext
	{
		/// <summary>
		/// Gets the controller creation options.
		/// </summary>
		PresentOptions PresentOptions { get; }

		/// <summary>
		/// Gets the controller creation arguments.
		/// </summary>
		PresentArgs PresentArgs { get; }

		/// <summary>
		/// Gets prent controller (if any).
		/// </summary>
		AppViewController ParentController { get; }

		/// <summary>
		/// Gets parent state.
		/// </summary>
		AppState ParentState { get; }

		/// <summary>
		/// tt
		/// </summary>
		AppView CreateView(AppViewController c);

		/// <summary>
		/// tt
		/// </summary>
		IAsyncOperation<AppViewController> PresentAsync(AppViewController parentController, Type controllerType, PresentOptions options, PresentArgs args);

		/// <summary>
		/// tt
		/// </summary>
		IAsyncOperation<AppViewController> PresentAsync(Type controllerType, PresentOptions options, PresentArgs args);

		/// <summary>
		/// tt
		/// </summary>
		IAsyncOperation DismissAsync(AppViewController c);

		/// <summary>
		/// tt
		/// </summary>
		IAsyncOperation DismissAsync();
	}
}
