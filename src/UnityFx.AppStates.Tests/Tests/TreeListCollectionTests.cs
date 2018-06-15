// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using NSubstitute;

namespace UnityFx.AppStates
{
	public class TreeListCollectionTests
	{
		#region data

		private class Node : TreeListNode<Node>
		{
			public Node() { }
			public Node(Node parent) : base(parent) { }
		}

		#endregion

		#region general

		[Fact]
		public void EmptyCollectionHasValidState()
		{
			// Arrange/Act
			var list = new TreeListCollection<Node>();

			// Assert
			AssertEmpty(list);
		}

		[Fact]
		public void Add_AddsElements()
		{
			// Arrange
			var sampleItems = new[] { new Node(), new Node() };
			var list = new TreeListCollection<Node>();

			// Act
			foreach (var item in sampleItems)
			{
				list.Add(item);
			}

			Assert.Equal(sampleItems[0], list.First);
			Assert.Equal(sampleItems[sampleItems.Length - 1], list.Last);
			Assert.Equal(sampleItems, list);
			Assert.Equal(sampleItems.Length, list.Count);
		}

		[Fact]
		public void Add_ThrowsOnNullElement()
		{
			// Arrange
			var list = new TreeListCollection<Node>();

			// Act/Assert
			Assert.Throws<ArgumentNullException>(() => list.Add(null));
		}

		[Fact]
		public void Remove_RemovesElements()
		{
			// Arrange
			var sampleItems = new[] { new Node(), new Node() };
			var list = new TreeListCollection<Node>();

			foreach (var item in sampleItems)
			{
				list.Add(item);
			}

			// Act
			foreach (var item in sampleItems)
			{
				list.Remove(item);
			}

			// Assert
			AssertEmpty(list);
		}

		[Fact]
		public void Remove_DoesNotThrowOnNullElements()
		{
			// Arrange
			var item = new Node();
			var list = new TreeListCollection<Node>() { item };

			// Act
			var result = list.Remove(null);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public void Remove_DoesNothingIfElementIsNotInCollection()
		{
			// Arrange
			var item = new Node();
			var item2 = new Node();
			var list = new TreeListCollection<Node>() { item };

			// Act
			var result = list.Remove(item2);

			// Assert
			Assert.False(result);
			Assert.Single(list);
			Assert.Equal(item, list.First);
			Assert.Equal(item, list.Last);
		}

		[Fact]
		public void Clear_RemovesAllElements()
		{
			// Arrange
			var list = new TreeListCollection<Node>() { new Node(), new Node() };

			// Act
			list.Clear();

			// Assert
			AssertEmpty(list);
		}

		[Fact]
		public void Contains_ReturnsTrueIfElementExists()
		{
			// Arrange
			var item = new Node();
			var item2 = new Node();
			var list = new TreeListCollection<Node>() { new Node(), item, new Node() };

			// Act
			var result1 = list.Contains(item);
			var result2 = list.Contains(item2);

			// Assert
			Assert.True(result1);
			Assert.False(result2);
		}

		[Fact]
		public void Contains_DoesNotThrowOnNullelement()
		{
			// Arrange
			var list = new TreeListCollection<Node>();

			// Act
			var result = list.Contains(null);

			// Assert
			Assert.False(result);
		}

		#endregion

		#region implementation

		private void AssertEmpty(TreeListCollection<Node> list)
		{
			Assert.Empty(list);
#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
			Assert.Equal(0, list.Count);
#pragma warning restore xUnit2013 // Do not use equality check to check for collection size.
			Assert.Null(list.First);
			Assert.Null(list.Last);
		}

		#endregion
	}
}
