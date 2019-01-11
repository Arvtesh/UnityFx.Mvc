// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A <see cref="MonoBehaviour"/> that implements <see cref="IComponent"/>.
	/// </summary>
	public abstract class ComponentBehaviour : DisposableBehaviour, IComponent
	{
		#region data

		private ISite _site;

		#endregion

		#region interface

		/// <summary>
		/// Gets the component name.
		/// </summary>
		protected string GetName()
		{
			if (_site != null)
			{
				return _site.Name;
			}

			return name;
		}

		/// <summary>
		/// Called when value of <see cref="Site"/> changes. Default implementation does nothing.
		/// </summary>
		/// <see cref="Site"/>
		protected virtual void OnSiteChanged()
		{
		}

		#endregion

		#region DisposableBehaviour

		/// <summary>
		/// Called when the object has been disposed.
		/// </summary>
		/// <seealso cref="Disposed"/>
		protected override void OnDisposed()
		{
			if (_site != null && _site.Container != null)
			{
				_site.Container.Remove(this);
			}

			if (Disposed != null)
			{
				Disposed(this, EventArgs.Empty);
			}
		}

		#endregion

		#region IComponent

		/// <summary>
		/// Gets or sets the <see cref="ISite"/> associated with the <see cref="IComponent"/>.
		/// </summary>
		public ISite Site
		{
			get
			{
				return _site;
			}
			set
			{
				// NOTE: This may be called during the object disposal (when IsDisposed is true), so do not call ThrowIfDisposed().
				if (_site != value)
				{
					_site = value;
					OnSiteChanged();
				}
			}
		}

		/// <summary>
		/// Represents the method that handles the dispose event of a component.
		/// </summary>
		public event EventHandler Disposed;

		#endregion
	}
}
