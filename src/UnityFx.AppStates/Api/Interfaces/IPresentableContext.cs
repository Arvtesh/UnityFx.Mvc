// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Context data for an <see cref="IPresentable"/> instance. The interface is basically a link between <see cref="IAppState"/> and child controllers.
	/// It is here for the sake of testability/explicit dependencies for <see cref="IPresentable"/> implementations.
	/// </summary>
	/// <seealso cref="IPresentable"/>
	public interface IPresentableContext
	{
		/// <summary>
		/// Gets the controller creation arguments.
		/// </summary>
		PresentArgs PresentArgs { get; }

		/// <summary>
		/// Gets parent state.
		/// </summary>
		IAppState ParentState { get; }

		/// <summary>
		/// Gets parent controller (if any).
		/// </summary>
		IPresentable ParentController { get; }

		/// <summary>
		/// Gets a view manager.
		/// </summary>
		IAppViewService ViewManager { get; }

		/// <summary>
		/// Presents a new state with a controller of the specified type.
		/// </summary>
		IAsyncOperation<IPresentable> PresentAsync(Type controllerType, PresentArgs args);

		/// <summary>
		/// Presents a new state with a controller of the specified type.
		/// </summary>
		IAsyncOperation<TController> PresentAsync<TController>(PresentArgs args) where TController : class, IPresentable;

		/// <summary>
		/// Dismisses the state with its controllers.
		/// </summary>
		IAsyncOperation DismissAsync();
	}
}
