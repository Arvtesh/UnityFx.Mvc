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

		private readonly TreeListCollection<Node> _list = new TreeListCollection<Node>();

		#endregion

		#region general

		[Fact]
		public void EmptyCollectionHasValidState()
		{
			// Assert
			Assert.Empty(_list);
#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
			Assert.Equal(0, _list.Count);
#pragma warning restore xUnit2013 // Do not use equality check to check for collection size.
			Assert.Null(_list.First);
			Assert.Null(_list.Last);
		}

		#endregion
	}
}
