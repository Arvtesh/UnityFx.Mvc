// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityFx.AppStates;
using UnityFx.AppStates.DependencyInjection;

namespace UnityFx.AppStates.Samples
{
	/// <summary>
	/// Application entry point.
	/// </summary>
	public class SimpleAppRoot : MonoBehaviour
	{
		#region data

		[SerializeField]
		private Transform _viewRoot = null;

		private ServiceProvider _serviceProvider;
		private IPrefabLoader _prefabLoader;
		private IAppViewService _viewManager;
		private IAppStateService _stateManager;

		#endregion

		#region MonoBehaviour

		private void Awake()
		{
			_serviceProvider = new ServiceProvider();
			_prefabLoader = new ResourcePrefabLoader();
			_viewManager = new PrefabViewService(_prefabLoader, _viewRoot);
			_stateManager = new AppStateService(_viewManager, _serviceProvider);
		}

		private void Start()
		{
			ConfigureServices(_serviceProvider);

			_stateManager.PresentAsync<MainMenuController>();
		}

		private void OnDestroy()
		{
			// Disposing _serviceProvider disposes all services registered.
			if (_serviceProvider != null)
			{
				_serviceProvider.Dispose();
				_serviceProvider = null;
			}
		}

		#endregion

		#region implementation

		private void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton(_prefabLoader);
			services.AddSingleton(_viewManager);
			services.AddSingleton(_stateManager);
		}

		#endregion
	}
}
