// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

namespace UnityFx.Mvc
{
	internal class ViewProxy : MonoBehaviour, ISite
	{
		#region data

		private IComponent _view;

		#endregion

		#region interface

		public Image Image { get; set; }
		public bool Exclusive { get; set; }

		#endregion

		#region MonoBehaviour
		#endregion

		#region ISite

		public IComponent Component
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
						_view.Site?.Container?.Remove(_view);
					}

					_view = value;

					if (_view != null)
					{
						_view.Site = this;

						if (_view is MonoBehaviour b)
						{
							if (b.transform.parent == null)
							{
								b.transform.SetParent(transform, false);
							}
						}
					}
				}
			}
		}

		public IContainer Container { get; set; }
		public bool DesignMode => false;
		public string Name { get => name; set => name = value; }

		#endregion

		#region IServiceProvider

		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(ISite))
			{
				return this;
			}

			if (Container is IServiceProvider sp)
			{
				return sp.GetService(serviceType);
			}

			return null;
		}


		#endregion

		#region implementation
		#endregion
	}
}
