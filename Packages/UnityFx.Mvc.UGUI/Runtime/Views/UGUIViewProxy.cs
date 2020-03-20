// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityFx.Mvc
{
	internal class UGUIViewProxy : MonoBehaviour
	{
		#region data

		private IView _view;

		#endregion

		#region interface

		public Image Image { get; set; }
		public bool Exclusive { get; set; }
		public bool Modal { get; set; }

		public IView View
		{
			get
			{
				return _view;
			}
			set
			{
				if (value != _view)
				{
					if (_view != null)
					{
						_view.Disposed -= OnViewDisposed;
					}

					_view = value;

					if (_view != null)
					{
						_view.Disposed += OnViewDisposed;

						if (_view is MonoBehaviour b)
						{
							if (b.transform.parent is null)
							{
								b.transform.SetParent(transform, false);
							}
						}
					}
				}
			}
		}

		#endregion

		#region MonoBehaviour

		private void OnDestroy()
		{
			if (_view is MonoBehaviour b && b && b.gameObject)
			{
				Destroy(b.gameObject);
			}
		}

		#endregion

		#region implementation

		private void OnViewDisposed(object sender, EventArgs args)
		{
			if (gameObject)
			{
				Destroy(gameObject);
			}
		}

		#endregion
	}
}
