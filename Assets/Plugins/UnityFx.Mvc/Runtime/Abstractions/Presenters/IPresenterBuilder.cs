// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Defines a class that provides the mechanisms to configure a <see cref="IPresenter"/> instance.
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
		/// Sets a <see cref="IViewFactory"/> instace to use.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="viewFactory"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the view factory is already set.</exception>
		/// <seealso cref="UseViewControllerFactory(IViewControllerFactory)"/>
		/// <seealso cref="Build"/>
		IPresenterBuilder UseViewFactory(IViewFactory viewFactory);

		/// <summary>
		/// Sets a <see cref="IViewControllerFactory"/> instace to use.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="viewControllerFactory"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the view controller factory is already set.</exception>
		/// <seealso cref="UseViewFactory(IViewFactory)"/>
		/// <seealso cref="Build"/>
		IPresenterBuilder UseViewControllerFactory(IViewControllerFactory viewControllerFactory);

		/// <summary>
		/// Adds a <see cref="PresentDelegate"/> to presenter middleware chain.
		/// </summary>
		/// <param name="presentDelegate">The middleware to add.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="presentDelegate"/> is <see langword="null"/>.</exception>
		/// <seealso cref="Build"/>
		IPresenterBuilder UsePresentDelegate(PresentDelegate presentDelegate);

		/// <summary>
		/// Creates a <see cref="MonoBehaviour"/>-based implementation of <see cref="IPresenter"/>. Can only be called once per builder instance.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown if presenter cannot be constructed (for instance, <see cref="IViewFactory"/> is not set and cannot be located).</exception>
		IPresentService Build();
	}
}
