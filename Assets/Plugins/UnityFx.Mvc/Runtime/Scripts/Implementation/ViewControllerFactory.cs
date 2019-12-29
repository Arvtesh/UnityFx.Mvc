// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Provides methods for creation and disposal of view controllers.
	/// </summary>
	/// <seealso cref="IViewController"/>
	public class ViewControllerFactory : IViewControllerFactory
	{
		#region data

		private readonly IServiceProvider _serviceProvider;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewControllerFactory"/> class.
		/// </summary>
		/// <param name="serviceProvider">The <see cref="IServiceProvider"/> instance to use.</param>
		public ViewControllerFactory(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		}

		#endregion

		#region IViewControllerFactory

		/// <summary>
		/// Creates a scoped <see cref="IServiceProvider"/> instance. Default implementation does not create scopes.
		/// </summary>
		/// <param name="serviceProvider">A <see cref="IServiceProvider"/> used to resolve controller dependencies.</param>
		/// <returns>A disposable scope created or <see langword="null"/>.</returns>
		public virtual IDisposable CreateScope(ref IServiceProvider serviceProvider)
		{
			return null;
		}

		/// <summary>
		/// Creates a new instance of <see cref="IViewController"/> and injects its dependencies.
		/// </summary>
		/// <param name="controllerType">Type of the controller to be created.</param>
		/// <param name="args">Additional arguments to use when resolving controller dependencies.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controllerType"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if <paramref name="controllerType"/> is not a valid controller type (for instance, <paramref name="controllerType"/> is abstract).</exception>
		/// <returns>The created controller instance.</returns>
		/// <seealso cref="Release(IViewController)"/>
		public virtual IViewController Create(Type controllerType, params object[] args)
		{
			if (controllerType is null)
			{
				throw new ArgumentNullException(nameof(controllerType));
			}

			return (IViewController)ActivatorUtilities.CreateInstance(_serviceProvider, controllerType, args);
		}

		/// <summary>
		/// Releases a controller after it has been dismissed. Default implementation calls <see cref="IDisposable.Dispose"/> if controller supports it.
		/// </summary>
		/// <param name="controller">The controller to be disposed.</param>
		/// <seealso cref="Create(Type, object[])"/>
		public virtual void Release(IViewController controller)
		{
			if (controller is IDisposable d)
			{
				d.Dispose();
			}
		}

		#endregion

		#region implementation
		#endregion
	}
}
