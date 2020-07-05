// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;
using UnityFx.Mvc;
using UnityFx.Mvc.Extensions;

namespace TestApp.Presentation
{
	/// <summary>
	/// Composition root. Initializes app and setups DI.
	/// </summary>
	/// <remarks>
	/// The app uses clean architecture as described in https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures (Onion view).
	/// 
	/// The codebase is split into layers:
	/// - Domain (namespace TestApp);
	/// - Infrastructure (namespace TestApp.Infrastructure);
	/// - Presentation (namespace TestApp.Presentation).
	/// 
	/// Domain layer shoud have no dependencies on other layers.
	/// Infrastructure layer depends on Domain.
	/// Presentation layer depends on Domain (but not Infrastructure).
	/// 
	/// Application business logic is contained in Domain. Domain also defines service interfaces and implementations of some of them.
	/// Infrastructure layer contains data access related code, DTOs and implementation of dedicated services.
	/// Presentation layer contains UI (views, controllers, view models, prefabs, etc).
	/// </remarks>
	public class AppRoot : MonoBehaviour, IServiceProvider
	{
		#region data

#pragma warning disable 0649

		[SerializeField]
		private GameObject _serviceRoot;
		[SerializeField]
		private UGUIMvcConfig _viewConfig;

#pragma warning restore 0649

		private IViewFactory _viewFactory;
		private IPresentService _presenter;

		private ServiceProvider _services;

		#endregion

		#region MonoBehaviour

		private void Awake()
		{
			if (!_serviceRoot)
			{
				_serviceRoot = new GameObject("Services");
				_serviceRoot.transform.SetParent(transform, false);
			}

			_viewFactory = new UGUIViewServiceBuilder(gameObject)
				.UseConfig(_viewConfig)
				.Build();

			_presenter = new PresenterBuilder(this, gameObject)
				.UseViewFactory(_viewFactory)
				.UseViewControllerBindings(_viewConfig)
				.UseErrorDelegate(OnPresentError)
				.Build();

			var serviceCollection = new ServiceCollection();
			ConfigureServices(serviceCollection);
			_services = serviceCollection.BuildServiceProvider();
		}

		private void Start()
		{
			_presenter.Present<AppController>();
		}

		private void OnDestroy()
		{
			_services.Dispose();
		}

		#endregion

		#region IServiceProvider

		object IServiceProvider.GetService(Type serviceType)
		{
			return _services.GetService(serviceType);
		}

		#endregion

		#region implementation

		private void ConfigureServices(IServiceCollection services)
		{
			// Presenter
			services.AddSingleton(_viewFactory);
			services.AddSingleton(_presenter);
			services.AddSingleton<IPresenter>(_presenter);

			// Web API
			services.AddSingleton(serviceProvider =>
			{
				return Infrastructure.Factory.CreateWebApi(_serviceRoot);
			});
		}

		protected Task InitializeAsync()
		{
			return Task.CompletedTask;
		}

		private void OnPresentError(Exception e)
		{
			Debug.LogException(e);
		}

		#endregion
	}

}
