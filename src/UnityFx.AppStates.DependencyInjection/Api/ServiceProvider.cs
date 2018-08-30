// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace UnityFx.AppStates.DependencyInjection
{
	/// <summary>
	/// Default implementation of <see cref="IServiceProvider"/>.
	/// </summary>
	public class ServiceProvider : IServiceProvider, IDisposable
	{
		#region data

		private Dictionary<Type, ServiceDescriptor> _services = new Dictionary<Type, ServiceDescriptor>();
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceProvider"/> class.
		/// </summary>
		/// <param name="services">A collection of service descriptors.</param>
		public ServiceProvider(IEnumerable<ServiceDescriptor> services)
		{
			foreach (var service in services)
			{
				if (_services.ContainsKey(service.ServiceType))
				{
					_services[service.ServiceType] = service;
				}
				else
				{
					_services.Add(service.ServiceType, service);
				}
			}
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

			if (_services.TryGetValue(serviceType, out var descriptor))
			{
				switch (descriptor.Lifetime)
				{
					case ServiceLifetime.Singleton:
						return GetSingletonService(descriptor);

					case ServiceLifetime.Scoped:
						return GetScopedService(descriptor);

					case ServiceLifetime.Transient:
						return GetTransientService(descriptor);

					default:
						throw new NotSupportedException(descriptor.Lifetime.ToString());
				}
			}

			throw new ServiceNotFoundException(serviceType);
		}

		#endregion

		#region IDisposable

		/// <inheritdoc/>
		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;

				foreach (var serviceDescriptor in _services.Values)
				{
					if (serviceDescriptor.ImplementationInstance is IDisposable service)
					{
						service.Dispose();
					}
				}

				_services.Clear();
			}

			GC.SuppressFinalize(this);
		}

		#endregion

		#region implementation

		private object CreateInstance(Type serviceType)
		{
			try
			{
				var constructors = serviceType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

				if (constructors.Length > 0)
				{
					var ctor = GetConstructor(constructors);
					var parameters = ctor.GetParameters();
					var args = new object[parameters.Length];

					for (var i = 0; i < args.Length; ++i)
					{
						args[i] = GetService(parameters[i].ParameterType);
					}

					return ctor.Invoke(args);
				}
				else
				{
					return Activator.CreateInstance(serviceType);
				}
			}
			catch (TargetInvocationException e)
			{
				throw e.InnerException;
			}
		}

		private ConstructorInfo GetConstructor(ConstructorInfo[] ctors)
		{
			// TODO: select the ctor
			return ctors[0];
		}

		private object GetSingletonService(ServiceDescriptor serviceDescriptor)
		{
			if (serviceDescriptor.ImplementationInstance != null)
			{
				return serviceDescriptor.ImplementationInstance;
			}
			else if (serviceDescriptor.ImplementationType != null)
			{
				var service = CreateInstance(serviceDescriptor.ImplementationType);
				serviceDescriptor.SetInstance(service);
				return service;
			}
			else if (serviceDescriptor.ImplementationFactory != null)
			{
				var service = serviceDescriptor.ImplementationFactory(this);
				serviceDescriptor.SetInstance(service);
				return service;
			}

			// Should not get here.
			Debug.Fail("Invalid service descriptor.");
			throw new InvalidOperationException();
		}

		private object GetTransientService(ServiceDescriptor serviceDescriptor)
		{
			if (serviceDescriptor.ImplementationType != null)
			{
				return CreateInstance(serviceDescriptor.ImplementationType);
			}
			else if (serviceDescriptor.ImplementationFactory != null)
			{
				return serviceDescriptor.ImplementationFactory(this);
			}

			// Should not get here.
			Debug.Fail("Invalid service descriptor.");
			throw new InvalidOperationException();
		}

		private object GetScopedService(ServiceDescriptor serviceDescriptor)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}
