// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A disposable <see cref="MonoBehaviour"/>.
	/// </summary>
	public abstract class DisposableMonoBehaviour : MonoBehaviour, IDisposable
	{
		#region data

		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Gets a value indicating whether the object is disposed.
		/// </summary>
		protected bool IsDisposed
		{
			get
			{
				return _disposed || !this;
			}
		}

		/// <summary>
		/// Throws an <see cref="ObjectDisposedException"/> if the object is disposed.
		/// </summary>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="Dispose(bool)"/>
		protected void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		/// <summary>
		/// Releases unmanaged resources used by the object.
		/// </summary>
		/// <param name="disposing">Should be <see langword="true"/> if the method is called from <see cref="Dispose()"/>; <see langword="false"/> otherwise.</param>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="ThrowIfDisposed"/>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				Destroy(gameObject);
			}
		}

		#endregion

		#region IDisposable

		/// <summary>
		/// Releases unmanaged resources used by the object.
		/// </summary>
		/// <seealso cref="Dispose(bool)"/>
		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		#endregion
	}
}
