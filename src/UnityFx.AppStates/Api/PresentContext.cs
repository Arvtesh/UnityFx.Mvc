// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Context data for an <see cref="IViewController"/> instance. The class is basically a link between <see cref="IAppState"/> and child controllers.
	/// It is here for the sake of testability/explicit dependencies for <see cref="IViewController"/> implementations.
	/// </summary>
	/// <seealso cref="IViewController"/>
	public class PresentContext
	{
		#region data

		private readonly IAppState _parentState;
		private readonly IViewController _parentController;
		private readonly PresentArgs _args;

		#endregion

		#region interface

		/// <summary>
		/// Gets the controller creation arguments.
		/// </summary>
		public PresentArgs PresentArgs => _args;

		/// <summary>
		/// Gets parent state.
		/// </summary>
		public IAppState ParentState => _parentState;

		/// <summary>
		/// Gets parent controller (if any).
		/// </summary>
		public IViewController ParentController => _parentController;

		/// <summary>
		/// Initializes a new instance of the <see cref="PresentContext"/> class.
		/// </summary>
		/// <param name="state">The state.</param>
		/// <param name="parentController">Parent controller instance.</param>
		/// <param name="args">Preset arguments.</param>
		public PresentContext(IAppState state, IViewController parentController, PresentArgs args)
		{
			_parentState = state;
			_parentController = parentController;
			_args = args;
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		public IAsyncOperation<IViewController> PresentAsync(Type controllerType, PresentArgs args)
		{
			return _parentState.PresentAsync(controllerType, args);
		}

		/// <summary>
		/// Presents a controller of the specified type.
		/// </summary>
		public IAsyncOperation<TController> PresentAsync<TController>(PresentArgs args) where TController : class, IViewController
		{
			return _parentState.PresentAsync<TController>(args);
		}

		/// <summary>
		/// Dismisses the state with its controllers.
		/// </summary>
		public IAsyncOperation DismissAsync()
		{
			return _parentState.DismissAsync();
		}

		#endregion
	}
}
