// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
#if !NET35
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace UnityFx.Mvc
{
	internal class PresentableStack : IPresentableStack
	{
		#region data

		private readonly TreeListCollection<PresentableProxy> _controllers;

		#endregion

		#region interface

		public PresentableStack(TreeListCollection<PresentableProxy> c)
		{
			_controllers = c;
		}

		#endregion

		#region IPresentableStack

		public int Count => _controllers.Count;

		public bool TryPeek(out IPresentable result)
		{
			if (_controllers.TryPeek(out var proxy))
			{
				result = proxy.Controller;
				return true;
			}

			result = null;
			return false;
		}

		public IPresentable Peek()
		{
			return _controllers.Peek().Controller;
		}

		#endregion

		#region IEnumerable

		public IEnumerator<IPresentable> GetEnumerator()
		{
			foreach (var c in _controllers)
			{
				yield return c.Controller;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (this as IEnumerable<IPresentable>).GetEnumerator();
		}

		#endregion
	}
}
