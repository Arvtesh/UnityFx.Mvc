// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityFx.Mvc
{
	partial class Presenter
	{
		private class ViewControllerCollection : IViewControllerCollection
		{
			#region data

			private readonly LinkedList<IPresentableProxy> _presentables;

			#endregion

			#region interface

			internal ViewControllerCollection(LinkedList<IPresentableProxy> presentables)
			{
				_presentables = presentables;
			}

			#endregion

			#region IViewControllerCollection

			public bool TryPeek(out IViewController controller)
			{
				controller = _presentables.Last?.Value.Controller;
				return controller != null;
			}

			public bool Contains(Type controllerType)
			{
				if (controllerType is null)
				{
					throw new ArgumentNullException(nameof(controllerType));
				}

				var p = _presentables.Last;

				while (p != null)
				{
					if (controllerType.IsAssignableFrom(p.Value.ControllerType))
					{
						return true;
					}

					p = p.Previous;
				}

				return false;
			}

			#endregion

			#region IReadOnlyCollection

			public int Count
			{
				get
				{
					var count = 0;

					foreach (var item in _presentables)
					{
						if (!item.IsDismissed)
						{
							++count;
						}
					}

					return count;
				}
			}

			#endregion

			#region IEnumerable

			public IEnumerator<IViewController> GetEnumerator()
			{
				var node = _presentables.First;

				while (node != null)
				{
					var p = node.Value;

					if (!p.IsDismissed)
					{
						yield return p.Controller;
					}

					node = node.Next;
				}
			}

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			#endregion
		}
	}
}
