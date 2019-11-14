// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

namespace UnityFx.Mvc
{
	partial class ViewFactory
	{
		#region interface

		public class ViewCollection : IList<IView>, IReadOnlyList<IView>
		{
			#region data

			private readonly ViewFactory _factory;

			#endregion

			#region interface

			internal ViewCollection(ViewFactory factory)
			{
				_factory = factory;
			}

			public IView[] ToArray()
			{
				var viewRoot = _factory.ViewRootTransform;
				var result = new IView[viewRoot.childCount];

				for (var i = 0; i < result.Length; ++i)
				{
					result[i] = viewRoot.GetChild(i).GetComponent<ViewProxy>()?.Component as IView;
				}

				return result;
			}

			#endregion

			#region IList

			public IView this[int index]
			{
				get
				{
					var viewRoot = _factory.ViewRootTransform;

					if (index < 0 || index >= viewRoot.childCount)
					{
						throw new ArgumentOutOfRangeException(nameof(index));
					}

					return viewRoot.GetChild(index).GetComponent<ViewProxy>()?.Component as IView;
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			public int IndexOf(IView view)
			{
				if (view != null)
				{
					var viewRoot = _factory.ViewRootTransform;

					for (var i = 0; i < viewRoot.childCount; ++i)
					{
						var proxy = viewRoot.GetChild(i).GetComponent<ViewProxy>();

						if (proxy && proxy.Component == view)
						{
							return i;
						}
					}
				}

				return -1;
			}

			public void Insert(int index, IView item)
			{
				throw new NotSupportedException();
			}

			public void RemoveAt(int index)
			{
				throw new NotSupportedException();
			}

			#endregion

			#region ICollection

			public int Count => _factory.ViewRootTransform.childCount;

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
					var viewRoot = _factory.ViewRootTransform;

					for (var i = 0; i < viewRoot.childCount; ++i)
					{
						var proxy = viewRoot.GetChild(i).GetComponent<ViewProxy>();

						if (proxy && proxy.Component == view)
						{
							return true;
						}
					}
				}

				return false;
			}

			public void CopyTo(IView[] array, int arrayIndex)
			{
				var viewRoot = _factory.ViewRootTransform;

				for (var i = 0; i < viewRoot.childCount; ++i)
				{
					array[arrayIndex + i] = viewRoot.GetChild(i).GetComponent<ViewProxy>()?.Component as IView;
				}
			}

			#endregion

			#region IEnumerable

			public IEnumerator<IView> GetEnumerator()
			{
				var viewRoot = _factory.ViewRootTransform;

				for (var i = 0; i < viewRoot.childCount; ++i)
				{
					var proxy = viewRoot.GetChild(i).GetComponent<ViewProxy>();
					yield return proxy?.Component as IView;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				var viewRoot = _factory.ViewRootTransform;

				for (var i = 0; i < viewRoot.childCount; ++i)
				{
					var proxy = viewRoot.GetChild(i).GetComponent<ViewProxy>();
					yield return proxy?.Component;
				}
			}

			#endregion
		}

		#endregion
	}
}
