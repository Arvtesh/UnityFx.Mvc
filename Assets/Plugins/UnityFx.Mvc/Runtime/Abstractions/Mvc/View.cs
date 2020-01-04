// Copyright (c) 2018-2020 Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A <see cref="MonoBehaviour"/>-based view.
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
		/// <typeparam name="TCommand">Type of the command.</typeparam>
		/// <param name="command">A generic command.</param>
		/// <seealso cref="NotifyCommand{TCommand, TArgs}(TCommand, TArgs)"/>
		/// <seealso cref="Command"/>
		protected void NotifyCommand<TCommand>(TCommand command)
		{
			Command?.Invoke(this, new CommandEventArgs<TCommand>(command));
		}

		/// <summary>
		/// Raises <see cref="Command"/> event.
		/// </summary>
		/// <typeparam name="TCommand">Type of the command.</typeparam>
		/// <typeparam name="TArgs">Type of the <paramref name="command"/> arguments.</typeparam>
		/// <param name="command">A generic command.</param>
		/// <param name="args">Command arguments.</param>
		/// <seealso cref="NotifyCommand{TCommand}(TCommand)"/>
		/// <seealso cref="Command"/>
		protected void NotifyCommand<TCommand, TArgs>(TCommand command, TArgs args)
		{
			Command?.Invoke(this, new CommandEventArgs<TCommand, TArgs>(command, args));
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
		/// <seealso cref="NotifyCommand{TCommand}(TCommand)"/>
		/// <seealso cref="NotifyCommand{TCommand, TArgs}(TCommand, TArgs)"/>
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
