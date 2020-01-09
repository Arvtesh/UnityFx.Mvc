﻿// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Factory of <see cref="IPresenter"/> instances.
	/// </summary>
	public static class PresenterFactory
	{
		/// <summary>
		/// Creates a <see cref="MonoBehaviour"/>-based implementation of <see cref="IPresenter"/>.
		/// </summary>
		/// <param name="serviceProvider">A <see cref="IServiceProvider"/> to use for dependency resolving.</param>
		/// <param name="go">A <see cref="GameObject"/> to attach presenter to.</param>
		/// <returns>Returns the presenter created.</returns>
		public static IPresentService CreatePresenter(IServiceProvider serviceProvider, GameObject go)
		{
			if (serviceProvider is null)
			{
				throw new ArgumentNullException(nameof(serviceProvider));
			}

			if (go is null)
			{
				throw new ArgumentNullException(nameof(go));
			}

			var viewFactory = serviceProvider.GetService(typeof(IViewFactory)) as IViewFactory;

			if (viewFactory is null)
			{
				viewFactory = go.GetComponentInChildren<IViewFactory>();

				if (viewFactory is null)
				{
					throw new InvalidOperationException($"No {typeof(IViewFactory).Name} instance registered. It should be accessible either via {nameof(serviceProvider)} or with {nameof(go.GetComponentInChildren)}().");
				}
			}

			var controllerFactory = serviceProvider.GetService(typeof(IViewControllerFactory)) as IViewControllerFactory;

			if (controllerFactory is null)
			{
				controllerFactory = new ViewControllerFactory(serviceProvider);
			}

			var presenter = go.AddComponent<Presenter>();
			presenter.Initialize(serviceProvider, viewFactory, controllerFactory);
			return presenter;
		}
	}
}
