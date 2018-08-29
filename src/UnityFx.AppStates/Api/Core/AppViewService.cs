// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic view service.
	/// </summary>
	public abstract class AppViewService : IAppViewService
	{
		#region data

		private readonly AppViewCollection _views = new AppViewCollection();
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// The service name.
		/// </summary>
		public const string ServiceName = "ViewManager";

		/// <summary>
		/// Creates a new view instance.
		/// </summary>
		protected abstract IAppView CreateView(string id, PresentOptions options);

		/// <summary>
		/// Releases unmanaged resources used by the service.
		/// </summary>
		/// <param name="disposing">Should be <see langword="true"/> if the method is called from <see cref="Dispose()"/>; <see langword="false"/> otherwise.</param>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="ThrowIfDisposed"/>
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				_disposed = true;

				if (disposing)
				{
					foreach (var view in _views)
					{
						view.Dispose();
					}

					_views.Clear();
				}
			}
		}

		/// <summary>
		/// Throws an <see cref="ObjectDisposedException"/> if the instance is already disposed.
		/// </summary>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="Dispose(bool)"/>
		protected void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(ServiceName);
			}
		}

		#endregion

		#region IAppViewService

		/// <inheritdoc/>
		public IAppViewCollection Views
		{
			get
			{
				return _views;
			}
		}

		/// <inheritdoc/>
		public IAppView CreateView(string id, IAppView insertAfter, PresentOptions options)
		{
			var result = CreateView(id, options);
			_views.Add(result, insertAfter);
			return result;
		}

		#endregion

		#region IDisposable

		/// <inheritdoc/>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		#region implementation
		#endregion
	}
}
