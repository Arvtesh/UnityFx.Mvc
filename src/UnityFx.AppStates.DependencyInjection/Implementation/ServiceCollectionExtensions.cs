// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;

namespace UnityFx.AppStates.DependencyInjection
{
	/// <summary>
	/// Extensions for <see cref="IServiceCollection"/>.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <paramref name="serviceType"/> and <paramref name="implementationType"/> and the <see cref="ServiceLifetime.Singleton"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <param name="serviceType">Type of the service.</param>
		/// <param name="implementationType">Type of the service implementation.</param>
		public static void AddSingleton(this IServiceCollection services, Type serviceType, Type implementationType)
		{
			services.Add(new ServiceDescriptor(serviceType, implementationType, ServiceLifetime.Singleton));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <paramref name="serviceType"/> and <paramref name="instance"/> and the <see cref="ServiceLifetime.Singleton"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <param name="serviceType">Type of the service.</param>
		/// <param name="instance">The service instance.</param>
		public static void AddSingleton(this IServiceCollection services, Type serviceType, object instance)
		{
			services.Add(new ServiceDescriptor(serviceType, instance));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <paramref name="serviceType"/> and <paramref name="implementationFactory"/> and the <see cref="ServiceLifetime.Singleton"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <param name="serviceType">Type of the service.</param>
		/// <param name="implementationFactory">Factory delegate for the service instances.</param>
		public static void AddSingleton(this IServiceCollection services, Type serviceType, Func<IServiceProvider, object> implementationFactory)
		{
			services.Add(new ServiceDescriptor(serviceType, implementationFactory, ServiceLifetime.Singleton));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <typeparamref name="TService"/> and <typeparamref name="TImplementation"/> and the <see cref="ServiceLifetime.Singleton"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <typeparam name="TService">Type of the service.</typeparam>
		/// <typeparam name="TImplementation">Type of the service implementation.</typeparam>
		public static void AddSingleton<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
		{
			services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <typeparamref name="TService"/> and <paramref name="instance"/> and the <see cref="ServiceLifetime.Singleton"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <typeparam name="TService">Type of the service.</typeparam>
		/// <param name="instance">The singleton instance.</param>
		public static void AddSingleton<TService>(this IServiceCollection services, TService instance) where TService : class
		{
			services.Add(new ServiceDescriptor(typeof(TService), instance));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <paramref name="serviceType"/> and <paramref name="implementationType"/> and the <see cref="ServiceLifetime.Transient"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <param name="serviceType">Type of the service.</param>
		/// <param name="implementationType">Type of the service implementation.</param>
		public static void AddTransient(this IServiceCollection services, Type serviceType, Type implementationType)
		{
			services.Add(new ServiceDescriptor(serviceType, implementationType, ServiceLifetime.Transient));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <paramref name="serviceType"/> and <paramref name="implementationFactory"/> and the <see cref="ServiceLifetime.Transient"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <param name="serviceType">Type of the service.</param>
		/// <param name="implementationFactory">Factory delegate for the service instances.</param>
		public static void AddTransient(this IServiceCollection services, Type serviceType, Func<IServiceProvider, object> implementationFactory)
		{
			services.Add(new ServiceDescriptor(serviceType, implementationFactory, ServiceLifetime.Transient));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <typeparamref name="TService"/> and <typeparamref name="TImplementation"/> and the <see cref="ServiceLifetime.Transient"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <typeparam name="TService">Type of the service.</typeparam>
		/// <typeparam name="TImplementation">Type of the service implementation.</typeparam>
		public static void AddTransient<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
		{
			services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Transient));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <paramref name="serviceType"/> and <paramref name="implementationType"/> and the <see cref="ServiceLifetime.Scoped"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <param name="serviceType">Type of the service.</param>
		/// <param name="implementationType">Type of the service implementation.</param>
		public static void AddScoped(this IServiceCollection services, Type serviceType, Type implementationType)
		{
			services.Add(new ServiceDescriptor(serviceType, implementationType, ServiceLifetime.Scoped));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <paramref name="serviceType"/> and <paramref name="implementationFactory"/> and the <see cref="ServiceLifetime.Scoped"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <param name="serviceType">Type of the service.</param>
		/// <param name="implementationFactory">Factory delegate for the service instances.</param>
		public static void AddScoped(this IServiceCollection services, Type serviceType, Func<IServiceProvider, object> implementationFactory)
		{
			services.Add(new ServiceDescriptor(serviceType, implementationFactory, ServiceLifetime.Scoped));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <typeparamref name="TService"/> and <typeparamref name="TImplementation"/> and the <see cref="ServiceLifetime.Scoped"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <typeparam name="TService">Type of the service.</typeparam>
		/// <typeparam name="TImplementation">Type of the service implementation.</typeparam>
		public static void AddScoped<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
		{
			services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Scoped));
		}
	}
}
