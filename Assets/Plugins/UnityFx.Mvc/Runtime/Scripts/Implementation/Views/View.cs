// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Default <see cref="MonoBehaviour"/>-based view.
	/// </summary>
	/// <seealso cref="ViewController"/>
	public class View : MonoBehaviour, IView
	{
		#region data

		private ISite _site;
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Gets a transform the <see cref="Site"/> is attached to (if any).
		/// </summary>
		protected Transform SiteTransform => (_site as UnityEngine.Component)?.transform;

		/// <summary>
		/// Gets a value indicating whether the view is disposed.
		/// </summary>
		/// <seealso cref="Dispose"/>
		/// <seealso cref="ThrowIfDisposed"/>
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
		/// <seealso cref="IsDisposed"/>
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
		/// <seealso cref="IsDisposed"/>
		/// <seealso cref="Dispose"/>
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

		/// <summary>
		/// Gets the <see cref="Transform"/> this view is attached to.
		/// </summary>
		public Transform Transform => transform;

		/// <summary>
		/// Gets or sets a value indicating whether the view <see cref="GameObject"/> is active.
		/// </summary>
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

		/// <summary>
		/// Raised when a user issues a command.
		/// </summary>
		/// <seealso cref="NotifyCommand(string, object)"/>
		public event EventHandler<CommandEventArgs> Command;

		#endregion

		#region IComponent

		/// <summary>
		/// Represents the method that handles the dispose event of a component.
		/// </summary>
		/// <seealso cref="Dispose"/>
		/// <seealso cref="OnDispose"/>
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

		/// <summary>
		/// Releases resources used by the view.
		/// </summary>
		/// <seealso cref="IsDisposed"/>
		/// <seealso cref="Disposed"/>
		/// <seealso cref="ThrowIfDisposed"/>
		/// <seealso cref="OnDispose"/>
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
