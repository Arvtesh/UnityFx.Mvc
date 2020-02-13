// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Default implementation of <see cref="IPresenterBuilder"/>.
	/// </summary>
	/// <seealso cref="UGUIViewFactoryBuilder"/>
	public sealed class PresenterBuilder : IPresenterBuilder
	{
		#region data

		private readonly IServiceProvider _serviceProvider;
		private readonly GameObject _gameObject;

		private Dictionary<string, object> _properties;
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

		#endregion

		#region IPresenterBuilder

		/// <inheritdoc/>
		public IServiceProvider ServiceProvider => _serviceProvider;

		/// <inheritdoc/>
		public IDictionary<string, object> Properties
		{
			get
			{
				if (_properties is null)
				{
					_properties = new Dictionary<string, object>();
				}

				return _properties;
			}
		}

		/// <inheritdoc/>
		public IPresenterBuilder UseViewFactory(IViewFactory viewFactory)
		{
			if (_viewFactory != null)
			{
				throw new InvalidOperationException();
			}

			_viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
			return this;
		}

		/// <inheritdoc/>
		public IPresenterBuilder UseViewControllerFactory(IViewControllerFactory viewControllerFactory)
		{
			if (_viewControllerFactory != null)
			{
				throw new InvalidOperationException();
			}

			_viewControllerFactory = viewControllerFactory ?? throw new ArgumentNullException(nameof(viewControllerFactory));
			return this;
		}

		/// <inheritdoc/>
		public IPresenterBuilder UsePresentDelegate(PresentDelegate presentDelegate)
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
			return this;
		}

		/// <inheritdoc/>
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

			var presenter = new Presenter(_serviceProvider, _viewFactory, _viewControllerFactory);
			presenter.SetMiddleware(_presentDelegates);
			return presenter;
		}

		#endregion
	}
}
