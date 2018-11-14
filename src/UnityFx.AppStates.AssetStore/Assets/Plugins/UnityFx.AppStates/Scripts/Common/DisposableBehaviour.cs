// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Mvc
{
	/// <summary>
	/// A disposable <see cref="MonoBehaviour"/>.
	/// </summary>
	public abstract class DisposableBehaviour : MonoBehaviour, IDisposable
	{
		#region data

		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Gets a value indicating whether the object is disposed.
		/// </summary>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="Dispose(bool)"/>
		/// <seealso cref="OnDisposed"/>
		/// <seealso cref="ThrowIfDisposed"/>
		protected bool IsDisposed
		{
			get
			{
				return _disposed;
			}
		}

		/// <summary>
		/// Throws an <see cref="ObjectDisposedException"/> if the object is disposed.
		/// </summary>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="Dispose(bool)"/>
		/// <seealso cref="OnDisposed"/>
		/// <seealso cref="IsDisposed"/>
		protected void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		/// <summary>
		/// Called when the object has been disposed. Default implementation does nothing.
		/// </summary>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="Dispose(bool)"/>
		/// <seealso cref="IsDisposed"/>
		/// <seealso cref="ThrowIfDisposed"/>
		protected virtual void OnDisposed()
		{
		}

		/// <summary>
		/// Releases unmanaged resources used by the object.
		/// </summary>
		/// <param name="disposing">Should be <see langword="true"/> if the method is called from <see cref="Dispose()"/>; <see langword="false"/> otherwise.</param>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="OnDisposed"/>
		/// <seealso cref="IsDisposed"/>
		/// <seealso cref="ThrowIfDisposed"/>
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				_disposed = true;

				if (disposing)
				{
					Destroy(gameObject);
					OnDisposed();
				}
			}
		}

		#endregion

		#region MonoBehaviour

		/// <summary>
		/// A <see cref="MonoBehaviour"/> destroy handler.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (!_disposed)
			{
				_disposed = true;
				OnDisposed();
			}
		}

		#endregion

		#region IDisposable

		/// <summary>
		/// Releases unmanaged resources used by the object.
		/// </summary>
		/// <seealso cref="Dispose(bool)"/>
		/// <seealso cref="OnDisposed"/>
		/// <seealso cref="IsDisposed"/>
		/// <seealso cref="ThrowIfDisposed"/>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
