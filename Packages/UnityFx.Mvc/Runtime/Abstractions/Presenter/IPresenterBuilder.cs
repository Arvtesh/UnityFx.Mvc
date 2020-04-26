// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
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
	public interface IPresenterBuilder
	{
		/// <summary>
		/// Gets the <see cref="IServiceProvider"/> that provides access to the application's service container.
		/// </summary>
		IServiceProvider ServiceProvider { get; }

		/// <summary>
		/// Gets a key/value collection that can be used to share data between middleware.
		/// </summary>
		IDictionary<string, object> Properties { get; }

		/// <summary>
		/// Sets a <see cref="IViewFactory"/> instace to use. The factory should be set before calling <see cref="Build"/>.
		/// </summary>
		/// <param name="viewFactory">A view factory to use.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="viewFactory"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if a view factory is already set.</exception>
		/// <seealso cref="UseViewControllerFactory(IViewControllerFactory)"/>
		/// <seealso cref="UseViewControllerBindings(IViewControllerBindings)"/>
		/// <seealso cref="UseEventSource(IPresenterEventSource)"/>
		/// <seealso cref="Build"/>
		IPresenterBuilder UseViewFactory(IViewFactory viewFactory);

		/// <summary>
		/// Sets a <see cref="IViewControllerFactory"/> instace to use. If not called, a default factory is used.
		/// </summary>
		/// <param name="viewControllerFactory">A view controller factory to use.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="viewControllerFactory"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if a view controller factory is already set.</exception>
		/// <seealso cref="UseViewControllerBindings(IViewControllerBindings)"/>
		/// <seealso cref="UseViewFactory(IViewFactory)"/>
		/// <seealso cref="Build"/>
		IPresenterBuilder UseViewControllerFactory(IViewControllerFactory viewControllerFactory);

		/// <summary>
		/// Sets a <see cref="IViewControllerBindings"/> instace to use. If not called, a default factory is used.
		/// </summary>
		/// <param name="viewControllerBindings">A view/controller bindings to use.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="viewControllerBindings"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if a view controller binding is already set.</exception>
		/// <seealso cref="UseViewControllerFactory(IViewControllerFactory)"/>
		/// <seealso cref="UseViewFactory(IViewFactory)"/>
		/// <seealso cref="Build"/>
		IPresenterBuilder UseViewControllerBindings(IViewControllerBindings viewControllerBindings);

		/// <summary>
		/// Sets an event source instance to use. If not called, a default provider is used.
		/// </summary>
		/// <param name="eventSource">An event source to use.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="eventSource"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if an event source is already set.</exception>
		/// <seealso cref="UseViewFactory(IViewFactory)"/>
		/// <seealso cref="Build"/>
		IPresenterBuilder UseEventSource(IPresenterEventSource eventSource);

#if UNITY_2019_3_OR_NEWER

		/// <summary>
		/// Sets a <see cref="UnityEngine.LowLevel.PlayerLoop"/>-based event source. Requires Unity 2019.3 or newer.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if an event source is already set, either with this method or with <see cref="UseEventSource(IPresenterEventSource)"/>.</exception>
		/// <seealso cref="UseEventSource(IPresenterEventSource)"/>
		/// <seealso cref="Build"/>
		IPresenterBuilder UsePlayerLoop();

#endif

		/// <summary>
		/// Adds a <see cref="PresentDelegate"/> to presenter middleware chain.
		/// </summary>
		/// <param name="presentDelegate">The middleware to add.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="presentDelegate"/> is <see langword="null"/>.</exception>
		/// <seealso cref="UseErrorDelegate(Action{Exception})"/>
		/// <seealso cref="Build"/>
		IPresenterBuilder UsePresentDelegate(PresentDelegate presentDelegate);

		/// <summary>
		/// Adds an error handler.
		/// </summary>
		/// <param name="errorDelegate">The delegate to be called on errors.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="errorDelegate"/> is <see langword="null"/>.</exception>
		/// <seealso cref="UsePresentDelegate(PresentDelegate)"/>
		/// <seealso cref="Build"/>
		IPresenterBuilder UseErrorDelegate(Action<Exception> errorDelegate);

		/// <summary>
		/// Builds a <see cref="IPresenter"/> instance.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if presenter cannot be constructed (for instance, <see cref="IViewFactory"/> is not set and cannot be located).</exception>
		/// <seealso cref="UseViewFactory(IViewFactory)"/>
		IPresentService Build();
	}
}
