// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using UnityEngine;

namespace UnityFx.AppStates.Sandbox
{
	public class AppRootBehaviour : MonoBehaviour, IServiceProvider
	{
		#region data

		[SerializeField]
		private Camera _camera;
		[SerializeField]
		private PrefabViewManagerBehaviour _viewManager;

		private TraceListener _traceListener;
		private IAppStateService _stateManager;

		#endregion

		#region MonoBehaviour

		private void Awake()
		{
			_traceListener = new UnityTraceListener();
			_stateManager = new AppStateService(this, _viewManager);
			_stateManager.Settings.TraceListeners.Add(_traceListener);
			_stateManager.Settings.TraceSwitch.Level = SourceLevels.All;
		}

		private void Start()
		{
			_stateManager.PresentAsync<MainMenuController>();
		}

		#endregion

		#region IServiceProvider

		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(IViewFactory))
			{
				return _viewManager;
			}
			else if (serviceType == typeof(IAppStateService))
			{
				return _stateManager;
			}
			else if (serviceType == typeof(IServiceProvider))
			{
				return this;
			}

			return null;
		}

		#endregion

		#region implementation
		#endregion
	}
}
