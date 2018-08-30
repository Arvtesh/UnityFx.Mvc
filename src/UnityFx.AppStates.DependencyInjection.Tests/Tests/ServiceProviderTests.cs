// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using Xunit;
using NSubstitute;

namespace UnityFx.AppStates.DependencyInjection
{
	public class ServiceProviderTests
	{
		[Fact]
		public void GetService_Throws_ServiceNotFoundException()
		{
			// Arrange
			var sp = new ServiceProvider();

			// Act/Assert
			Assert.Throws<ServiceNotFoundException>(() => sp.GetService(typeof(IEnumerable)));
		}

		[Theory]
		[InlineData(typeof(SelfReferenceClass))]
		[InlineData(typeof(CrossReferenceClass1))]
		[InlineData(typeof(CrossReferenceClass2))]
		[InlineData(typeof(InvalidService))]
		public void GetService_Throws_ServiceConstructorResolutionException(Type type)
		{
			// Arrange
			var sp = new ServiceProvider();
			sp.Add(new ServiceDescriptor(type, type, ServiceLifetime.Transient));

			// Act/Assert
			Assert.Throws<ServiceConstructorResolutionException>(() => sp.GetService(type));
		}

		[Fact]
		public void GetService_CreatesOnlyOneSingletonInstance()
		{
			// Arrange
			var sp = new ServiceProvider();
			sp.Add(new ServiceDescriptor(typeof(IEnumerable), typeof(ArrayList), ServiceLifetime.Singleton));

			// Act
			var instance1 = sp.GetService(typeof(IEnumerable));
			var instance2 = sp.GetService(typeof(IEnumerable));

			// Assert
			Assert.NotNull(instance1);
			Assert.NotNull(instance2);
			Assert.Same(instance1, instance2);
		}

		[Fact]
		public void GetService_CreatesManyTransientInstances()
		{
			// Arrange
			var sp = new ServiceProvider();
			sp.Add(new ServiceDescriptor(typeof(IEnumerable), typeof(ArrayList), ServiceLifetime.Transient));

			// Act
			var instance1 = sp.GetService(typeof(IEnumerable));
			var instance2 = sp.GetService(typeof(IEnumerable));

			// Assert
			Assert.NotNull(instance1);
			Assert.NotNull(instance2);
			Assert.NotSame(instance1, instance2);
		}

		[Fact]
		public void GetService_ResolvesMultipleCtors()
		{
			// Arrange
			var sp = new ServiceProvider();
			sp.Add(new ServiceDescriptor(typeof(MultiCtorClass), typeof(MultiCtorClass), ServiceLifetime.Transient));

			// Act
			var instance = sp.GetService(typeof(MultiCtorClass));

			// Assert
			Assert.NotNull(instance);
		}
	}
}
