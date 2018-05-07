// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// Creation options for <see cref="AppView"/>.
	/// </summary>
	[Flags]
	public enum AppViewOptions
	{
		/// <summary>
		/// Default options.
		/// </summary>
		None,
	}

	/// <summary>
	/// A generic composite view.
	/// </summary>
	public abstract class AppView : IComponentContainer, IDisposable
	{
		#region data

		[Flags]
		private enum Flags
		{
			Visible = 1,
			Enabled = 2,
			Exclusive = 4,
			Loading = 8,
			Loaded = 16,
			Disposed = 32,
		}

		private readonly string _name;
		private readonly AppViewOptions _options;
		private readonly AppView _parent;

		private IAsyncOperation _loadOp;
		private Flags _flags;

		#endregion

		#region interface

		/// <summary>
		/// Gets the view identifier.
		/// </summary>
		public string Id => _name;

		/// <summary>
		/// Gets the view creation options.
		/// </summary>
		public AppViewOptions Options => _options;

		/// <summary>
		/// Gets a parent view (if any).
		/// </summary>
		public AppView Parent => _parent;

		/// <summary>
		/// Gets a value indicating whether the view is loaded or not.
		/// </summary>
		public bool IsLoaded => (_flags & Flags.Loaded) != 0;

		/// <summary>
		/// Gets a value indicating whether the view is loaded or not.
		/// </summary>
		public bool IsLoading => (_flags & Flags.Loading) != 0;

		/// <summary>
		/// Gets or sets a value indicating whether the view is visible.
		/// </summary>
		public bool Visible
		{
			get
			{
				return (_flags & Flags.Visible) != 0;
			}
			set
			{
				var visible = (_flags & Flags.Visible) != 0;

				if (visible != value)
				{
					SetVisible(value);
					_flags ^= Flags.Visible;
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the view is enabled.
		/// </summary>
		public bool Enabled
		{
			get
			{
				return (_flags & Flags.Enabled) != 0;
			}
			set
			{
				var enabled = (_flags & Flags.Enabled) != 0;

				if (enabled != value)
				{
					SetEnabled(value);
					_flags ^= Flags.Enabled;
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppView"/> class.
		/// </summary>
		/// <param name="name">Name of the view.</param>
		/// <param name="options"></param>
		protected AppView(string name, AppViewOptions options)
		{
			_name = name;
			_options = options;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppView"/> class.
		/// </summary>
		/// <param name="name">Name of the view.</param>
		/// <param name="parent"></param>
		/// <param name="options"></param>
		protected AppView(string name, AppView parent, AppViewOptions options)
		{
			_name = name;
			_options = options;
			_parent = parent;
		}

		/// <summary>
		/// Initiates loading content of the view.
		/// </summary>
		public IAsyncOperation Load()
		{
			if ((_flags & Flags.Loaded) == 0)
			{
				if (_loadOp == null)
				{
					_flags |= Flags.Loading;
					_loadOp = LoadContent(_name);
					_loadOp.AddCompletionCallback(op =>
					{
						_loadOp = null;
						_flags &= ~Flags.Loading;
						_flags &= Flags.Loaded;
					});
				}

				return _loadOp;
			}

			return AsyncResult.CompletedOperation;
		}

		/// <summary>
		/// Throws an <see cref="ObjectDisposedException"/> if the controller is disposed.
		/// </summary>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="Dispose(bool)"/>
		protected void ThrowIfDisposed()
		{
			if ((_flags & Flags.Disposed) != 0)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		/// <summary>
		/// tt
		/// </summary>
		protected abstract void SetVisible(bool visible);

		/// <summary>
		/// tt
		/// </summary>
		protected abstract void SetEnabled(bool visible);

		/// <summary>
		/// tt
		/// </summary>
		protected abstract IAsyncOperation LoadContent(string id);

		/// <summary>
		/// Releases unmanaged resources used by the view.
		/// </summary>
		/// <param name="disposing">Should be <see langword="true"/> if the method is called from <see cref="Dispose()"/>; <see langword="false"/> otherwise.</param>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="ThrowIfDisposed"/>
		protected virtual void Dispose(bool disposing)
		{
			_flags &= Flags.Disposed;
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

		/// <summary>
		/// Releases unmanaged resources used by the view.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
