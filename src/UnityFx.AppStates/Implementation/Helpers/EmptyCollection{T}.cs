// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityFx.AppStates
{
	internal class EmptyCollection<T> : IReadOnlyCollection<T>
	{
		private static EmptyCollection<T> _empty = new EmptyCollection<T>();

		public static IReadOnlyCollection<T> Instance => _empty;

		public int Count => 0;
		public IEnumerator<T> GetEnumerator() => Enumerable.Empty<T>().GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => Enumerable.Empty<T>().GetEnumerator();
	}
}
