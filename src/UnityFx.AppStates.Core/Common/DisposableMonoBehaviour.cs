// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Implementation of <see cref="IDisposable"/>.
	/// </summary>
	internal class DisposableMonoBehaviour : MonoBehaviour, IDisposable
	{
		#region data

		private bool _disposed;

		#endregion

		#region interface

		protected void SetDisposed()
		{
			_disposed = true;
		}

		protected void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(name);
			}
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;

				if (this)
				{
					Destroy(gameObject);
				}
			}
		}

		#endregion
	}
}
