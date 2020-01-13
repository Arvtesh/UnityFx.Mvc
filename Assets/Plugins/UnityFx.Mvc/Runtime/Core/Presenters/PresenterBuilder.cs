// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Present delegate that is called right before presenting a controller.
	/// </summary>
	public delegate Task PresentDelegate(IViewControllerInfo controllerInfo);

	/// <summary>
	/// Factory of <see cref="IPresenter"/> instances.
	/// </summary>
	public sealed class PresenterBuilder
	{
		#region data

		private readonly IServiceProvider _serviceProvider;
		private readonly GameObject _gameObject;

		private List<PresentDelegate> _presentDelegates;
		private IViewControllerFactory _viewControllerFactory;
		private IViewFactory _viewFactory;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="PresenterBuilder"/> class.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="serviceProvider"/> or <paramref name="go"/> is <see langword="null"/>.</exception>
		public PresenterBuilder(IServiceProvider serviceProvider, GameObject go)
		{
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
			_gameObject = go ?? throw new ArgumentNullException(nameof(go));
		}

		/// <summary>
		/// Sets a <see cref="IViewFactory"/> instace to use.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="viewFactory"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the view factory is already set.</exception>
		public void UseViewFactory(IViewFactory viewFactory)
		{
			if (_viewFactory != null)
			{
				throw new InvalidOperationException();
			}

			_viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
		}

		/// <summary>
		/// Sets a <see cref="IViewControllerFactory"/> instace to use.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="viewControllerFactory"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the view controller factory is already set.</exception>
		public void UseViewControllerFactory(IViewControllerFactory viewControllerFactory)
		{
			if (_viewControllerFactory != null)
			{
				throw new InvalidOperationException();
			}

			_viewControllerFactory = viewControllerFactory ?? throw new ArgumentNullException(nameof(viewControllerFactory));
		}

		/// <summary>
		/// Adds a <see cref="PresentDelegate"/> to controller middleware chain.
		/// </summary>
		/// <param name="presentDelegate">The middleware to add.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="presentDelegate"/> is <see langword="null"/>.</exception>
		public void Use(PresentDelegate presentDelegate)
		{
			if (presentDelegate is null)
			{
				throw new ArgumentNullException(nameof(presentDelegate));
			}

			if (_presentDelegates is null)
			{
				_presentDelegates = new List<PresentDelegate>();
			}

			_presentDelegates.Add(presentDelegate);
		}

		/// <summary>
		/// Creates a <see cref="MonoBehaviour"/>-based implementation of <see cref="IPresenter"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if presenter cannot be constructed (for instance, <see cref="IViewFactory"/> is not set and cannot be located).</exception>
		public IPresentService Build()
		{
			if (_viewFactory is null)
			{
				_viewFactory = _serviceProvider.GetService(typeof(IViewFactory)) as IViewFactory;

				if (_viewFactory is null)
				{
					_viewFactory = _gameObject.GetComponentInChildren<IViewFactory>();

					if (_viewFactory is null)
					{
						throw new InvalidOperationException($"No {typeof(IViewFactory).Name} instance registered. It should be accessible either via IServiceProvider or with GameObject.GetComponentInChildren().");
					}
				}
			}

			if (_viewControllerFactory is null)
			{
				_viewControllerFactory = _serviceProvider.GetService(typeof(IViewControllerFactory)) as IViewControllerFactory;

				if (_viewControllerFactory is null)
				{
					_viewControllerFactory = new ViewControllerFactory(_serviceProvider);
				}
			}

			var presenter = _gameObject.AddComponent<Presenter>();
			presenter.Initialize(_serviceProvider, _viewFactory, _viewControllerFactory);
			presenter.SetMiddleware(_presentDelegates);
			return presenter;
		}

		#endregion
	}
}
