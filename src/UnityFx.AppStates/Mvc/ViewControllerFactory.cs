// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Default implementation of <see cref="IViewControllerFactory"/>.
	/// </summary>
	/// <seealso cref="ViewController"/>
	public abstract class ViewControllerFactory : IViewControllerFactory
	{
		#region data

		private readonly IServiceProvider _serviceProvider;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewControllerFactory"/> class.
		/// </summary>
		/// <param name="serviceProvider">The <see cref="IServiceProvider"/> instance to use.</param>
		protected ViewControllerFactory(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		}

		#endregion

		#region IViewControllerFactory

		/// <inheritdoc/>
		public virtual IDisposable CreateControllerScope(ref IServiceProvider serviceProvider)
		{
			return null;
		}

		/// <inheritdoc/>
		public IViewController CreateController(Type controllerType, params object[] args)
		{
			return (IViewController)ActivatorUtilities.CreateInstance(_serviceProvider, controllerType, args);
		}

		#endregion

		#region implementation
		#endregion
	}
}
