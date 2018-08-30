// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using Xunit;

namespace UnityFx.AppStates.DependencyInjection
{
	public class ServiceCollectionTests
	{
		[Fact]
		public void Add_AddsElements()
		{
			// Arrange
			var list = new ServiceCollection();
			var item = new ServiceDescriptor(typeof(IEnumerable), new ArrayList());

			// Act
			list.Add(item);

			// Assert
			Assert.Contains(item, list);
		}

		[Fact]
		public void Remove_RemovesElements()
		{
			// Arrange
			var list = new ServiceCollection();
			var item = new ServiceDescriptor(typeof(IEnumerable), new ArrayList());

			// Act
			list.Add(item);
			var result = list.Remove(item);

			// Assert
			Assert.Empty(list);
			Assert.True(result);
		}
	}
}
