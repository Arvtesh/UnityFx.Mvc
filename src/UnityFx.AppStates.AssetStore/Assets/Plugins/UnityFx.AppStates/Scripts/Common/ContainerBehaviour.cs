// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Prefab view manager.
	/// </summary>
	public abstract class ContainerBehaviour : DisposableBehaviour, IContainer, IServiceProvider
	{
		#region data

		private class Site : ISite
		{
			private readonly IComponent _component;
			private readonly ContainerBehaviour _container;
			private string _name;

			internal Site(IComponent component, ContainerBehaviour container, string name)
			{
				_component = component;
				_container = container;
				_name = name;
			}

			public IComponent Component
			{
				get
				{
					return _component;
				}
			}

			public IContainer Container
			{
				get
				{
					return _container;
				}
			}

			public bool DesignMode
			{
				get
				{
					return false;
				}
			}

			public string Name
			{
				get
				{
					return _name;
				}
				set
				{
					if (value == null || _name == null || !value.Equals(_name))
					{
						_container.ValidateName(_component, value);
						_name = value;
					}
				}
			}

			public object GetService(Type serviceType)
			{
				if (serviceType == typeof(ISite))
				{
					return this;
				}

				return _container.GetService(serviceType);
			}
		}

		private List<ISite> _sites = new List<ISite>();
		private ComponentCollection _components;

		#endregion

		#region interface

		/// <summary>
		/// Gets number of components in the container.
		/// </summary>
		protected int ComponentCount
		{
			get
			{
				return _sites.Count;
			}
		}

		/// <summary>
		/// Adds the specified component to the <see cref="IContainer"/> at the specified index, and assigns a name to the component.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="component"/> is <see langword="null"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the container is disposed.</exception>
		protected bool Add(IComponent component, string name, int index)
		{
			ThrowIfDisposed();

			if (component == null)
			{
				throw new ArgumentNullException("view");
			}

			var site = component.Site;

			if (site == null || !ReferenceEquals(site.Container, this))
			{
				ValidateName(component, name);

				if (OnAddComponent(component, index))
				{
					if (site != null)
					{
						site.Container.Remove(component);
					}

					site = CreateSite(component, name);
					component.Site = site;

					_sites.Add(site);
					_components = null;

					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Called when a component is about to be added.
		/// </summary>
		protected virtual bool OnAddComponent(IComponent component, int index)
		{
			return true;
		}

		/// <summary>
		/// Called when a component has been removed.
		/// </summary>
		protected virtual void OnRemoveComponent(IComponent component)
		{
		}

		/// <summary>
		/// Validates the component name specified.
		/// </summary>
		protected virtual void ValidateName(IComponent component, string name)
		{
		}

		/// <summary>
		/// Creates a new <see cref="ISite"/> instance.
		/// </summary>
		protected virtual ISite CreateSite(IComponent component, string name)
		{
			return new Site(component, this, name);
		}

		#endregion

		#region IContainer

		/// <summary>
		/// Gets all the components in the <see cref="IContainer"/>.
		/// </summary>
		public ComponentCollection Components
		{
			get
			{
				if (_components == null)
				{
					var result = new IComponent[_sites.Count];

					for (var i = 0; i < result.Length; ++i)
					{
						result[i] = _sites[i].Component;
					}

					_components = new ComponentCollection(result);
				}

				return _components;
			}
		}

		/// <summary>
		/// Adds the specified component to the <see cref="IContainer"/> at the end of the list.
		/// </summary>
		/// <param name="component">The component to add.</param>
		/// <seealso cref="Add(IComponent, string)"/>
		/// <seealso cref="Remove(IComponent)"/>
		public void Add(IComponent component)
		{
			Add(component, null, _sites.Count);
		}

		/// <summary>
		/// Adds the specified component to the <see cref="IContainer"/> at the end of the list, and assigns a name to the component.
		/// </summary>
		/// <param name="component">The component to add.</param>
		/// <param name="name">The unique, case-sensitive name to assign to the component.-or- null that leaves the component unnamed.</param>
		/// <seealso cref="Add(IComponent)"/>
		/// <seealso cref="Remove(IComponent)"/>
		public void Add(IComponent component, string name)
		{
			Add(component, name, _sites.Count);
		}

		/// <summary>
		/// Removes a component from the <see cref="IContainer"/>.
		/// </summary>
		/// <param name="component">The component to remove.</param>
		/// <seealso cref="Add(IComponent)"/>
		/// <seealso cref="Add(IComponent, string)"/>
		public void Remove(IComponent component)
		{
			if (component != null && !IsDisposed)
			{
				var site = component.Site;

				if (site != null && ReferenceEquals(site.Container, this))
				{
					component.Site = null;

					_sites.Remove(site);
					_components = null;

					OnRemoveComponent(component);
				}
			}
		}

		#endregion

		#region IServiceProvider

		/// <summary>
		/// Gets the service object of the specified type.
		/// </summary>
		public virtual object GetService(Type serviceType)
		{
			if (serviceType == typeof(IContainer))
			{
				return this;
			}

			return null;
		}

		#endregion

		#region implementation
		#endregion
	}
}
