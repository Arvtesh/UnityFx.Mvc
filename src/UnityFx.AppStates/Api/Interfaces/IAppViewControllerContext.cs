// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Context data for an <see cref="AppViewController"/> instance. The interface is basically a link between <see cref="AppState"/> and child controllers.
	/// It is here for the sake of testability/explicit dependencies for <see cref="AppViewController"/>.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
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
		/// Creates a view for the specified controller.
		/// </summary>
		AppView CreateView(AppViewController c);

		/// <summary>
		/// Presents a child controller of the specified type.
		/// </summary>
		IAsyncOperation<AppViewController> PresentAsync(AppViewController parentController, Type controllerType, PresentOptions options, PresentArgs args);

		/// <summary>
		/// Presents a new state with a controller of the specified type.
		/// </summary>
		IAsyncOperation<AppViewController> PresentAsync(Type controllerType, PresentOptions options, PresentArgs args);

		/// <summary>
		/// Dismisses the specified child controller.
		/// </summary>
		IAsyncOperation DismissAsync(AppViewController c);

		/// <summary>
		/// Dismisses the state with its controllers.
		/// </summary>
		IAsyncOperation DismissAsync();
	}
}
