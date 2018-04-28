// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace UnityFx.AppStates
{
	internal class AppStateManagerShared : AppStateServiceSettings
	{
		#region data

		private readonly SynchronizationContext _synchronizationContext;
		private readonly IAppStateControllerFactory _controllerFactory;
		private readonly IAppStateViewManager _viewManager;
		private readonly IAppStateTransitionManager _transitionManager;
		private readonly IServiceProvider _serviceProvider;

		#endregion

		#region interface

		internal SynchronizationContext SynchronizationContext => _synchronizationContext;
		internal IAppStateControllerFactory ControllerFactory => _controllerFactory;
		internal IAppStateViewManager ViewManager => _viewManager;
		internal IAppStateTransitionManager TransitionManager => _transitionManager;
		internal IServiceProvider ServiceProvider => _serviceProvider;

		internal AppStateManagerShared(
			SynchronizationContext syncContext,
			IAppStateControllerFactory controllerFactory,
			IAppStateViewManager viewManager,
			IAppStateTransitionManager transitionManager,
			IServiceProvider services)
		{
			_synchronizationContext = syncContext;
			_controllerFactory = controllerFactory;
			_viewManager = viewManager;
			_transitionManager = transitionManager;
			_serviceProvider = services;
		}

		internal AppStateController CreateController(AppState state, Type controllerType)
		{
			Debug.Assert(state != null);
			Debug.Assert(controllerType != null);

			return _controllerFactory.CreateController(controllerType, state);
		}

		internal IAppStateView CreateView(AppState state)
		{
			Debug.Assert(state != null);

			return _viewManager.CreateView(state.Id, state.GetPrevView());
		}

		#endregion

		#region implementation
		#endregion
	}
}
