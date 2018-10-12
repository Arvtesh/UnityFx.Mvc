// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using UnityEngine;

namespace UnityFx.AppStates
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
		#endregion

		#region DisposableBehaviour

		/// <summary>
		/// Called when the object has been disposed.
		/// </summary>
		protected override void OnDisposed()
		{
			base.OnDisposed();

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
				_site = value;
			}
		}

		/// <summary>
		/// Represents the method that handles the dispose event of a component.
		/// </summary>
		public event EventHandler Disposed;

		#endregion
	}
}
