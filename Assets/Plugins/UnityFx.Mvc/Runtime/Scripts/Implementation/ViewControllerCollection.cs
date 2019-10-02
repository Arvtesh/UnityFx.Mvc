// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityFx.Mvc
{
	internal class ViewControllerCollection : IViewControllerCollection
	{
		#region data

		private readonly TreeListCollection<PresentResult> _controllers;

		#endregion

		#region interface

		public ViewControllerCollection(TreeListCollection<PresentResult> c)
		{
			_controllers = c;
		}

		#endregion

		#region IViewControllerCollection

		public int Count => _controllers.Count;

		public bool TryPeek(out IViewController result)
		{
			if (_controllers.TryPeek(out var proxy))
			{
				result = proxy.Controller;
				return true;
			}

			result = null;
			return false;
		}

		public IViewController Peek()
		{
			return _controllers.Peek().Controller;
		}

		#endregion

		#region IEnumerable

		public IEnumerator<IViewController> GetEnumerator()
		{
			foreach (var c in _controllers)
			{
				yield return c.Controller;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (this as IEnumerable<IViewController>).GetEnumerator();
		}

		#endregion
	}
}
