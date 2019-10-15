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
		#endregion

		#region interface

		public Image Image { get; set; }
		public bool Exclusive { get; set; }

		#endregion

		#region MonoBehaviour
		#endregion

		#region ISite

		public IComponent Component { get; set; }
		public IContainer Container { get; set; }
		public bool DesignMode => false;
		public string Name { get; set; }

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
