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
	public class ServiceProvider : IServiceCollection, IServiceProvider, IDisposable
	{
		#region data

		private Dictionary<Type, ServiceDescriptor> _services = new Dictionary<Type, ServiceDescriptor>();
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceProvider"/> class.
		/// </summary>
		public ServiceProvider()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceProvider"/> class.
		/// </summary>
		/// <param name="serviceDescriptors">A collection of service descriptors.</param>
		public ServiceProvider(IEnumerable<ServiceDescriptor> serviceDescriptors)
		{
			foreach (var serviceDescriptor in serviceDescriptors)
			{
				Add(serviceDescriptor);
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

			return GetService(serviceType, null);
		}

		#endregion

		#region ICollection

		/// <inheritdoc/>
		public int Count => _services.Count;

		/// <inheritdoc/>
		public bool IsReadOnly => false;

		/// <inheritdoc/>
		public void Add(ServiceDescriptor item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			if (_services.ContainsKey(item.ServiceType))
			{
				_services[item.ServiceType] = item;
			}
			else
			{
				_services.Add(item.ServiceType, item);
			}
		}

		/// <inheritdoc/>
		public bool Remove(ServiceDescriptor item)
		{
			if (item != null)
			{
				return _services.Remove(item.ServiceType);
			}

			return false;
		}

		/// <inheritdoc/>
		public bool Contains(ServiceDescriptor item)
		{
			if (item != null)
			{
				return _services.ContainsKey(item.ServiceType);
			}

			return false;
		}

		/// <inheritdoc/>
		public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
		{
			_services.Values.CopyTo(array, arrayIndex);
		}

		/// <inheritdoc/>
		public void Clear()
		{
			_services.Clear();
		}

		#endregion

		#region IEnumerable

		/// <inheritdoc/>
		public IEnumerator<ServiceDescriptor> GetEnumerator() => _services.Values.GetEnumerator();

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator() => _services.Values.GetEnumerator();

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

		private object CreateInstance(Type serviceType, ICollection<Type> callerTypes)
		{
			try
			{
				var constructors = serviceType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

				if (constructors.Length > 0)
				{
					// Add the service type to a set of caller types. We have to maintain it to avoid loops like
					// A requires B, B requires A.
					if (callerTypes == null)
					{
						callerTypes = new HashSet<Type>() { serviceType };
					}
					else
					{
						callerTypes.Add(serviceType);
					}

					var ctor = GetConstructor(constructors, callerTypes);

					if (ctor != null)
					{
						var parameters = ctor.GetParameters();
						var args = new object[parameters.Length];

						for (var i = 0; i < args.Length; ++i)
						{
							args[i] = GetService(parameters[i].ParameterType, callerTypes);
						}

						return ctor.Invoke(args);
					}
					else
					{
						throw new ServiceConstructorResolutionException(serviceType);
					}
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

		private ConstructorInfo GetConstructor(ConstructorInfo[] ctors, ICollection<Type> callerTypes)
		{
			// Select the first ctor having all arguments registered.
			foreach (var ctor in ctors)
			{
				var argumentsValidated = true;

				foreach (var arg in ctor.GetParameters())
				{
					// Make sure the argument type is registered in _services (so we can create it) and no construction loops are detected.
					if (!_services.ContainsKey(arg.ParameterType) || (callerTypes != null && callerTypes.Contains(arg.ParameterType)))
					{
						argumentsValidated = false;
						break;
					}
				}

				if (argumentsValidated)
				{
					return ctor;
				}
			}

			return null;
		}

		private object GetService(Type serviceType, ICollection<Type> callerTypes)
		{
			if (_services.TryGetValue(serviceType, out var descriptor))
			{
				switch (descriptor.Lifetime)
				{
					case ServiceLifetime.Singleton:
						return GetSingletonService(descriptor, callerTypes);

					case ServiceLifetime.Scoped:
						return GetScopedService(descriptor, callerTypes);

					case ServiceLifetime.Transient:
						return GetTransientService(descriptor, callerTypes);

					default:
						throw new NotSupportedException(descriptor.Lifetime.ToString());
				}
			}

			throw new ServiceNotFoundException(serviceType);
		}

		private object GetSingletonService(ServiceDescriptor serviceDescriptor, ICollection<Type> callerTypes)
		{
			if (serviceDescriptor.ImplementationInstance != null)
			{
				return serviceDescriptor.ImplementationInstance;
			}
			else if (serviceDescriptor.ImplementationType != null)
			{
				var service = CreateInstance(serviceDescriptor.ImplementationType, callerTypes);
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

		private object GetTransientService(ServiceDescriptor serviceDescriptor, ICollection<Type> callerTypes)
		{
			if (serviceDescriptor.ImplementationType != null)
			{
				return CreateInstance(serviceDescriptor.ImplementationType, callerTypes);
			}
			else if (serviceDescriptor.ImplementationFactory != null)
			{
				return serviceDescriptor.ImplementationFactory(this);
			}

			// Should not get here.
			Debug.Fail("Invalid service descriptor.");
			throw new InvalidOperationException();
		}

		private object GetScopedService(ServiceDescriptor serviceDescriptor, ICollection<Type> callerTypes)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}
