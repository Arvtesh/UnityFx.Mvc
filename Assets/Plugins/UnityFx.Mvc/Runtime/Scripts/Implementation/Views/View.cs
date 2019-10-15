// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace UnityFx.Mvc
{
	public class View : MonoBehaviour, IView
	{
		#region data

		private ISite _site;
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Gets a value indicating whether the view is disposed.
		/// </summary>
		/// <seealso cref="Dispose"/>
		protected bool IsDisposed => _disposed;

		/// <summary>
		/// Raises <see cref="Command"/> event.
		/// </summary>
		/// <param name="commandId">Name of the command.</param>
		/// <param name="args">Command arguments.</param>
		/// <seealso cref="Command"/>
		protected void NotifyCommand(string commandId, object args = null)
		{
			Command?.Invoke(this, new CommandEventArgs(commandId, args));
		}

		/// <summary>
		/// Throws an <see cref="ObjectDisposedException"/> if the controller is disposed.
		/// </summary>
		/// <seealso cref="Dispose"/>
		protected void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		/// <summary>
		/// Called when the view is disposed.
		/// </summary>
		/// <seealso cref="ThrowIfDisposed"/>
		protected virtual void OnDispose()
		{
		}

		#endregion

		#region MonoBehaviour

		private void OnDestroy()
		{
			Dispose();
		}

		#endregion

		#region IView

		public Transform Transform => transform;

		public bool Enabled
		{
			get
			{
				return gameObject.activeSelf;
			}
			set
			{
				ThrowIfDisposed();
				gameObject.SetActive(value);
			}
		}

		#endregion

		#region INotifyCommand

		public event EventHandler<CommandEventArgs> Command;

		#endregion

		#region IComponent

		/// <summary>
		/// Represents the method that handles the dispose event of a component.
		/// </summary>
		public event EventHandler Disposed;

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

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				_site?.Container?.Remove(this);

				try
				{
					OnDispose();
				}
				finally
				{
					Disposed?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		#endregion

		#region implementation
		#endregion
	}
}
