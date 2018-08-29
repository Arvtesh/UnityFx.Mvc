// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityFx.AppStates.DependencyInjection
{
	/// <summary>
	/// Default implementation of <see cref="IServiceProvider"/>.
	/// </summary>
	public class ServiceProvider : IServiceProvider, IDisposable
	{
		#region data

		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceProvider"/> class.
		/// </summary>
		/// <param name="services">A collection of service descriptors.</param>
		public ServiceProvider(IEnumerable<ServiceDescriptor> services)
		{
			// TODO
		}

		#endregion

		#region IServiceProvider

		/// <inheritdoc/>
		public object GetService(Type serviceType)
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			throw new NotImplementedException();
		}

		#endregion

		#region IDisposable

		/// <inheritdoc/>
		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;

				// TODO
			}

			GC.SuppressFinalize(this);
		}

		#endregion

		#region implementation
		#endregion
	}
}
