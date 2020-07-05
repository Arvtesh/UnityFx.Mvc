using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace TestApp
{
	public abstract class DisposableMonoBehaviour : MonoBehaviour, IDisposable
	{
		#region data

		private bool _disposed;

		#endregion

		#region interface

		protected bool IsDisposed => _disposed;

		protected virtual void OnDispose()
		{
		}

		protected void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		#endregion

		#region MonoBehaviour

		private void OnDestroy()
		{
			Dispose();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				OnDispose();
			}
		}

		#endregion

		#region implementation
		#endregion
	}
}
