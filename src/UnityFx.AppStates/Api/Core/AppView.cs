// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class AppView : TreeListNode<IAppView>, IAppView
	{
		#region data

		private static int _idCounter;

		private readonly string _id;
		private readonly string _resourceId;
		private readonly PresentOptions _options;

		private AsyncLazy _loadOp;
		private bool _visible;
		private bool _enabled;
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="AppView"/> class.
		/// </summary>
		public AppView(string id, PresentOptions options)
		{
			_id = Utility.GetNextId("_view", ref _idCounter);
			_resourceId = id;
			_options = options;
			_loadOp.OperationFactory = LoadInternal;
		}

		/// <summary>
		/// Sets view visibility flag.
		/// </summary>
		protected abstract void SetVisible(bool visible);

		/// <summary>
		/// Sets view input enabled flag.
		/// </summary>
		protected abstract void SetEnabled(bool enabled);

		/// <summary>
		/// Loads the view content.
		/// </summary>
		protected abstract IAsyncOperation LoadContent(string resourceId);

		/// <summary>
		/// Releases unmanaged resources used by the service.
		/// </summary>
		/// <param name="disposing">Should be <see langword="true"/> if the method is called from <see cref="Dispose()"/>; <see langword="false"/> otherwise.</param>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="ThrowIfDisposed"/>
		protected virtual void Dispose(bool disposing)
		{
			_disposed = true;
		}

		/// <summary>
		/// Throws an <see cref="ObjectDisposedException"/> if the instance is already disposed.
		/// </summary>
		/// <seealso cref="Dispose()"/>
		protected void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(_id);
			}
		}

		#endregion

		#region IAppView

		/// <inheritdoc/>
		public string Id => _id;

		/// <inheritdoc/>
		public bool IsLoaded => _loadOp.IsCompletedSuccessfully;

		/// <inheritdoc/>
		public bool IsLoading => _loadOp.IsStarted && !_loadOp.IsCompleted;

		/// <inheritdoc/>
		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				if (_loadOp)
				{
					SetVisible(value);
				}

				_visible = value;
			}
		}

		/// <inheritdoc/>
		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				if (_loadOp)
				{
					SetEnabled(value);
				}

				_enabled = value;
			}
		}

		/// <inheritdoc/>
		public IAsyncOperation Load()
		{
			return _loadOp.StartOrUpdate();
		}

		#endregion

		#region IComponentContainer

		/// <inheritdoc/>
		public abstract TComponent GetComponent<TComponent>();

		/// <inheritdoc/>
		public abstract TComponent GetComponentRecursive<TComponent>();

		/// <inheritdoc/>
		public abstract TComponent[] GetComponents<TComponent>();

		/// <inheritdoc/>
		public abstract TComponent[] GetComponentsRecursive<TComponent>();

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

		private IAsyncOperation LoadInternal()
		{
			return LoadContent(_resourceId);
		}

		#endregion
	}
}
