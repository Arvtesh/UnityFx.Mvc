// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Defines a class that provides the mechanisms to create and configure a presenter.
	/// </summary>
	/// <remarks>
	/// This interface defines fluent configuration API for <see cref="IPresenter"/>. It is designed to be
	/// similar to ASP.NET <see href="https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.builder.iapplicationbuilder"/>.
	/// </remarks>
	/// <example>
	/// The following code demonstrates configuring the app presenter:
	/// <code>
	/// private void Configure(IPresenterBuilder presenterBuilder)
	/// {
	///		presenterBuilder
	///			.UseViewFactory(_myViewFactory)
	///			.UseViewControllerFactory(_myCustomControllerFactory)	// If not called, default controller factory is used.
	///			.UsePresentDelegate((presenter, controllerInfo) =>
	///				{
	///					// Log the present operation.
	///					Debug.Log($"Present {controllerInfo.ControllerType.Name}.");
	///					return Task.CompletedTask;
	///				})
	///			.UsePresentDelegate((presenter, controllerInfo) =>
	///				{
	///					// Make sure that user has been authorized.
	///					var userInfo = (UserInfo)presenter.ServiceProvider.GetService(typeof(UserInfo));
	///					
	///					if (!userInfo.IsAuthorized)
	///					{
	///						throw new InvalidOperationException();
	///					}
	/// 
	///					return Task.CompletedTask;
	///				});
	/// }
	/// </code>
	/// </example>
	/// <seealso cref="IPresenter"/>
	/// <seealso href="https://en.wikipedia.org/wiki/Builder_pattern"/>
	public sealed class PresenterBuilder
	{
		#region data

		private readonly IServiceProvider _serviceProvider;
		private readonly GameObject _gameObject;
		private readonly bool _useServiceProvider;

		private IViewFactory _viewFactory;
		private IViewControllerFactory _viewControllerFactory;
		private IViewControllerBindings _viewControllerBindings;
		private IPresenterEventSource _eventSource;
		private List<PresentDelegate> _presentDelegates;
		private Action<Exception> _errorDelegate;

		private Presenter _presenter;

		#endregion

		#region interface

		/// <summary>
		/// Gets the <see cref="IServiceProvider"/> that provides access to the application's service container.
		/// </summary>
		public IServiceProvider ServiceProvider => _serviceProvider;

		/// <summary>
		/// Initializes a new instance of the <see cref="PresenterBuilder"/> class.
		/// </summary>
		/// <param name="serviceProvider">A service provider to resolve controller dependencies.</param>
		/// <param name="gameObject">A <see cref="GameObject"/> to attach presenter to. Can be <see langword="null"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceProvider"/> is <see langword="null"/>.</exception>
		public PresenterBuilder(IServiceProvider serviceProvider, GameObject gameObject)
		{
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
			_gameObject = gameObject;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PresenterBuilder"/> class.
		/// </summary>
		/// <param name="serviceProvider">A service provider to resolve controller dependencies.</param>
		/// <param name="gameObject">A <see cref="GameObject"/> to attach presenter to. Can be <see langword="null"/>.</param>
		/// <param name="useServiceProvider">If <see langword="true"/>, the class might use <see cref="ServiceProvider"/> before presenter is constructed.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceProvider"/> is <see langword="null"/>.</exception>
		public PresenterBuilder(IServiceProvider serviceProvider, GameObject gameObject, bool useServiceProvider)
		{
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
			_gameObject = gameObject;
			_useServiceProvider = useServiceProvider;
		}

		/// <summary>
		/// Sets a <see cref="IViewFactory"/> instace to use. The factory should be set before calling <see cref="Build"/>.
		/// </summary>
		/// <param name="viewFactory">A view factory to use.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="viewFactory"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if a view factory is already set.</exception>
		/// <seealso cref="UseViewControllerFactory(IViewControllerFactory)"/>
		/// <seealso cref="UseBindings(IViewControllerBindings)"/>
		/// <seealso cref="UseEventSource(IPresenterEventSource)"/>
		/// <seealso cref="Build"/>
		public PresenterBuilder UseViewFactory(IViewFactory viewFactory)
		{
			if (_viewFactory != null)
			{
				throw new InvalidOperationException();
			}

			_viewFactory = viewFactory ?? throw new ArgumentNullException(nameof(viewFactory));
			return this;
		}

		/// <summary>
		/// Sets a <see cref="IViewControllerFactory"/> instace to use. If not called, a default factory is used.
		/// </summary>
		/// <param name="viewControllerFactory">A view controller factory to use.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="viewControllerFactory"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if a view controller factory is already set.</exception>
		/// <seealso cref="UseBindings(IViewControllerBindings)"/>
		/// <seealso cref="UseViewFactory(IViewFactory)"/>
		/// <seealso cref="Build"/>
		public PresenterBuilder UseViewControllerFactory(IViewControllerFactory viewControllerFactory)
		{
			if (_viewControllerFactory != null)
			{
				throw new InvalidOperationException();
			}

			_viewControllerFactory = viewControllerFactory ?? throw new ArgumentNullException(nameof(viewControllerFactory));
			return this;
		}

		/// <summary>
		/// Sets a <see cref="IViewControllerBindings"/> instace to use. If not called, a default factory is used.
		/// </summary>
		/// <param name="viewControllerBindings">A view/controller bindings to use.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="viewControllerBindings"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if a view controller binding is already set.</exception>
		/// <seealso cref="UseViewControllerFactory(IViewControllerFactory)"/>
		/// <seealso cref="UseViewFactory(IViewFactory)"/>
		/// <seealso cref="Build"/>
		public PresenterBuilder UseBindings(IViewControllerBindings viewControllerBindings)
		{
			if (_viewControllerBindings != null)
			{
				throw new InvalidOperationException();
			}

			_viewControllerBindings = viewControllerBindings ?? throw new ArgumentNullException(nameof(viewControllerBindings));
			return this;
		}

		/// <summary>
		/// Sets an event source instance to use. If not called, a default provider is used.
		/// </summary>
		/// <param name="eventSource">An event source to use.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="eventSource"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if an event source is already set.</exception>
		/// <seealso cref="UseViewFactory(IViewFactory)"/>
		/// <seealso cref="Build"/>
		public PresenterBuilder UseEventSource(IPresenterEventSource eventSource)
		{
			if (_eventSource != null)
			{
				throw new InvalidOperationException();
			}

			_eventSource = eventSource ?? throw new ArgumentNullException(nameof(eventSource));
			return this;
		}

		/// <summary>
		/// Sets a <see cref="UnityEngine.LowLevel.PlayerLoop"/>-based event source. Requires Unity 2019.3 or newer.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if an event source is already set, either with this method or with <see cref="UseEventSource(IPresenterEventSource)"/>.</exception>
		/// <seealso cref="UseEventSource(IPresenterEventSource)"/>
		/// <seealso cref="Build"/>
		public PresenterBuilder UsePlayerLoop()
		{
			if (_eventSource != null)
			{
				throw new InvalidOperationException();
			}

			_eventSource = new PlayerLoopEventSource();
			return this;
		}

		/// <summary>
		/// Adds a <see cref="PresentDelegate"/> to presenter middleware chain.
		/// </summary>
		/// <param name="presentDelegate">The middleware to add.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="presentDelegate"/> is <see langword="null"/>.</exception>
		/// <seealso cref="UseErrorDelegate(Action{Exception})"/>
		/// <seealso cref="Build"/>
		public PresenterBuilder UsePresentDelegate(PresentDelegate presentDelegate)
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

		/// <summary>
		/// Adds an error handler.
		/// </summary>
		/// <param name="errorDelegate">The delegate to be called on errors.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="errorDelegate"/> is <see langword="null"/>.</exception>
		/// <seealso cref="UsePresentDelegate(PresentDelegate)"/>
		/// <seealso cref="Build"/>
		public PresenterBuilder UseErrorDelegate(Action<Exception> errorDelegate)
		{
			if (errorDelegate is null)
			{
				throw new ArgumentNullException(nameof(errorDelegate));
			}

			_errorDelegate += errorDelegate;
			return this;
		}

		/// <summary>
		/// Builds a <see cref="IPresenter"/> instance.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if presenter cannot be constructed (for instance, <see cref="IViewFactory"/> is not set and cannot be located).</exception>
		/// <seealso cref="UseViewFactory(IViewFactory)"/>
		public IPresentService Build()
		{
			if (_presenter is null)
			{
				if (_eventSource is null && _gameObject is null)
				{
					throw new InvalidOperationException("No event source is set. Presenter requires update notifications in order to function properly.");
				}

				if (_viewFactory is null)
				{
					if (_useServiceProvider)
					{
						_viewFactory = _serviceProvider.GetService(typeof(IViewFactory)) as IViewFactory;
					}

					if (_viewFactory is null)
					{
						_viewFactory = _gameObject?.GetComponentInChildren<IViewFactory>();

						if (_viewFactory is null)
						{
							throw new InvalidOperationException($"No {typeof(IViewFactory).Name} instance registered. It should be accessible either via IServiceProvider or with GameObject.GetComponentInChildren().");
						}
					}
				}

				if (_viewControllerFactory is null)
				{
					if (_useServiceProvider)
					{
						_viewControllerFactory = _serviceProvider.GetService(typeof(IViewControllerFactory)) as IViewControllerFactory;
					}

					if (_viewControllerFactory is null)
					{
						_viewControllerFactory = new ViewControllerFactory(_serviceProvider);
					}
				}

				if (_viewControllerBindings is null)
				{
					if (_useServiceProvider)
					{
						_viewControllerBindings = _serviceProvider.GetService(typeof(IViewControllerBindings)) as IViewControllerBindings;
					}

					if (_viewControllerBindings is null)
					{
						_viewControllerBindings = new ViewControllerBindings();
					}
				}

				_presenter = new Presenter(_serviceProvider, _viewFactory, _viewControllerFactory, _viewControllerBindings, _eventSource);
				_presenter.SetMiddleware(_presentDelegates);
				_presenter.SetErrorHandler(_errorDelegate);

				_gameObject?.AddComponent<PresenterBehaviour>().Initialize(_presenter);
			}

			return _presenter;
		}

		#endregion

		#region implementation
		#endregion
	}
}
