// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace UnityFx.AppStates
{
	internal class AppStateManagerShared : IAppStateServiceSettings
	{
		#region data

		private readonly TraceSource _console = new TraceSource("AppStates");
		private readonly SynchronizationContext _synchronizationContext;
		private readonly IAppStateControllerFactory _controllerFactory;
		private readonly IAppStateViewManager _viewManager;
		private readonly IAppStateTransitionManager _transitionManager;
		private readonly IServiceProvider _serviceProvider;

		private string _deeplinkScheme;
		private string _deeplinkDomain = string.Empty;

		#endregion

		#region interface

		internal TraceSource TraceSource => _console;
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

		#region IAppStateServiceSettings

		public SourceSwitch TraceSwitch { get => _console.Switch; set => _console.Switch = value; }
		public TraceListenerCollection TraceListeners => _console.Listeners;

		public int MaxNumberOfPendingOperations
		{
			get
			{
				return 0;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string DeeplinkDomain
		{
			get
			{
				return _deeplinkDomain;
			}
			set
			{
				_deeplinkDomain = value ?? throw new ArgumentNullException(nameof(value));
			}
		}

		public string DeeplinkScheme
		{
			get
			{
				return _deeplinkScheme;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				if (!Uri.CheckSchemeName(value))
				{
					throw new ArgumentException("Invalid scheme value.", nameof(value));
				}

				_deeplinkScheme = value;
			}
		}

		#endregion

		#region implementation
		#endregion
	}
}
