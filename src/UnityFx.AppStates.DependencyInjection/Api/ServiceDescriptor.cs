// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates.DependencyInjection
{
	/// <summary>
	/// Describes a service with its service type, implementation, and lifetime.
	/// </summary>
	/// <seealso cref="ServiceCollection"/>
	/// <seealso cref="IServiceCollection"/>
	/// <seealso cref="IServiceProvider"/>
	public class ServiceDescriptor
	{
		#region data

		private readonly Type _serviceType;
		private readonly Type _implementationType;
		private readonly ServiceLifetime _serviceLifetime;
		private readonly object _serviceInstance;

		#endregion

		#region interface

		/// <summary>
		/// Gets the service type.
		/// </summary>
		public Type ServiceType => _serviceType;

		/// <summary>
		/// Gets the service type.
		/// </summary>
		public Type ImplementationType => _implementationType;

		/// <summary>
		/// Gets the singleton service instance.
		/// </summary>
		public object ImplementationInstance => _serviceInstance;

		/// <summary>
		/// Gets the service lifetime.
		/// </summary>
		public ServiceLifetime Lifetime => _serviceLifetime;

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceDescriptor"/> class with the specified <paramref name="instance"/> as a <see cref="ServiceLifetime.Singleton"/>.
		/// </summary>
		/// <param name="serviceType">Service type.</param>
		/// <param name="instance">Service singleton instance.</param>
		public ServiceDescriptor(Type serviceType, object instance)
		{
			_serviceType = serviceType;
			_serviceLifetime = ServiceLifetime.Singleton;
			_serviceInstance = instance;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceDescriptor"/> class with the specified <paramref name="instance"/> as a <see cref="ServiceLifetime.Singleton"/>.
		/// </summary>
		/// <param name="serviceType">Service type.</param>
		/// <param name="implementationType">Service implementation type.</param>
		/// <param name="lifetime">Service lifetime.</param>
		public ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
		{
			_serviceType = serviceType;
			_implementationType = implementationType;
			_serviceLifetime = lifetime;
		}

		#endregion

		#region implementation
		#endregion
	}
}
