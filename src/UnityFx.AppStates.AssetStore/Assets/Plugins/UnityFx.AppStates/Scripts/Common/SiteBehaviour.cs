// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using UnityEngine;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A <see cref="MonoBehaviour"/>-base implementation of <see cref="ISite"/>.
	/// </summary>
	public class SiteBehaviour : MonoBehaviour, ISite
	{
		#region data

		private IComponent _component;
		private IContainer _container;
		private IServiceProvider _serviceProvider;

		#endregion

		#region interface
		#endregion

		#region MonoBehaviour
		#endregion

		#region ISite

		/// <summary>
		/// Gets the component associated with the <see cref="ISite"/>.
		/// </summary>
		public IComponent Component
		{
			get
			{
				if (_component == null)
				{
					_component = GetComponentInChildren<IComponent>(true);
				}

				return _component;
			}
		}

		/// <summary>
		/// Gets the <see cref="IContainer"/> associated with the <see cref="ISite"/>.
		/// </summary>
		public IContainer Container
		{
			get
			{
				if (_container == null && transform.parent)
				{
					_container = transform.parent.GetComponent<IContainer>();
				}

				return _container;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the component is in design mode.
		/// </summary>
		public bool DesignMode
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets or sets the name of the component associated with the <see cref="ISite"/>.
		/// </summary>
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		#endregion

		#region IServiceProvider

		/// <summary>
		/// Gets the service object of the specified type.
		/// </summary>
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(ISite))
			{
				return this;
			}

			if (_serviceProvider == null && transform.parent)
			{
				_serviceProvider = transform.parent.GetComponentInParent<IServiceProvider>();
			}

			if (_serviceProvider != null)
			{
				return _serviceProvider.GetService(serviceType);
			}

			return null;
		}

		#endregion
	}
}
