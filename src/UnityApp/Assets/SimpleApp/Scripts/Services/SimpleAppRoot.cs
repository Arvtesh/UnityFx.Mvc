// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityFx.AppStates;
using UnityFx.DependencyInjection;

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

		#endregion

		#region MonoBehaviour

		private void Awake()
		{
			var serviceCollection = new ServiceCollection();
			ConfigureServices(serviceCollection);
			_serviceProvider = serviceCollection.BuildServiceProvider();
		}

		private void Start()
		{
			_serviceProvider.GetService<IAppStateService>().PresentAsync<MainMenuController>();
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
			var prefabLoader = new ResourcePrefabLoader();
			var viewManager = new PrefabViewService(prefabLoader, _viewRoot);

			services.AddSingleton<IPrefabLoader>(prefabLoader);
			services.AddSingleton<IAppViewService>(viewManager);
			services.AddSingleton<IAppStateService, AppStateService>();
		}

		#endregion
	}
}
