// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityFx.Mvc
{
	partial class UGUIViewFactory
	{
		#region interface

		public class ViewCollection : ICollection<IView>, IReadOnlyCollection<IView>
		{
			#region data

			private readonly UGUIViewFactory _factory;

			#endregion

			#region interface

			internal ViewCollection(UGUIViewFactory factory)
			{
				_factory = factory;
			}

			public IView[] ToArray()
			{
				var result = new IView[Count];
				CopyTo(result, 0);
				return result;
			}

			#endregion

			#region ICollection

			public int Count
			{
				get
				{
					var viewRoots = _factory.ViewRoots;

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

			public bool IsReadOnly => true;

			public void Add(IView view)
			{
				throw new NotSupportedException();
			}

			public bool Remove(IView item)
			{
				throw new NotSupportedException();
			}

			public void Clear()
			{
				throw new NotSupportedException();
			}

			public bool Contains(IView view)
			{
				if (view != null)
				{
					var viewRoots = _factory.ViewRoots;

					if (viewRoots != null)
					{
						foreach (var viewRoot in viewRoots)
						{
							for (var i = 0; i < viewRoot.childCount; ++i)
							{
								var proxy = viewRoot.GetChild(i).GetComponent<UGUIViewProxy>();

								if (proxy && proxy.View == view)
								{
									return true;
								}
							}
						}
					}
				}

				return false;
			}

			public void CopyTo(IView[] array, int arrayIndex)
			{
				var viewRoots = _factory.ViewRoots;

				if (viewRoots != null)
				{
					foreach (var viewRoot in viewRoots)
					{
						for (var i = 0; i < viewRoot.childCount; ++i)
						{
							array[arrayIndex + i] = viewRoot.GetChild(i).GetComponent<UGUIViewProxy>()?.View;
						}
					}
				}
			}

			#endregion

			#region IEnumerable

			public IEnumerator<IView> GetEnumerator()
			{
				var viewRoots = _factory.ViewRoots;

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
