// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityFx.Mvc
{
	partial class UGUIViewService
	{
		#region interface

		public class ViewCollection : IReadOnlyCollection<IView>
		{
			#region data

			private readonly UGUIViewService _factory;

			#endregion

			#region interface

			internal ViewCollection(UGUIViewService factory)
			{
				_factory = factory;
			}

			#endregion

			#region IReadOnlyCollection

			public int Count
			{
				get
				{
					var viewRoots = _factory.Layers;

					if (viewRoots != null)
					{
						var result = 0;

						foreach (var viewRoot in viewRoots)
						{
							result += viewRoot.childCount;
						}

						return result;
					}

					return 0;
				}
			}

			#endregion

			#region IEnumerable

			public IEnumerator<IView> GetEnumerator()
			{
				var viewRoots = _factory.Layers;

				if (viewRoots != null)
				{
					foreach (var viewRoot in viewRoots)
					{
						for (var i = 0; i < viewRoot.childCount; ++i)
						{
							var proxy = viewRoot.GetChild(i).GetComponent<UGUIViewProxy>();
							yield return proxy?.View;
						}
					}
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			#endregion
		}

		#endregion
	}
}
