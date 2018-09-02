// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic view controller. It is recommended to use this class as base for all other controllers.
	/// Note that minimal controller implementation should inherit <see cref="IPresentable"/>.
	/// </summary>
	public class AppViewController : IPresenter, IPresentable, IPresentableEvents
	{
		#region data

		private static int _idCounter;

		private readonly IPresentableContext _context;
		private readonly IAppState _state;
		private readonly IAppView _view;

		private readonly string _id;
		private readonly string _resourceId;
		private readonly PresentArgs _presentArgs;

		private IAsyncOperation _dismissOp;
		private bool _active;
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Gets the parent state.
		/// </summary>
		protected IAppState ParentState => _state;

		/// <summary>
		/// Gets the controller creation arguments.
		/// </summary>
		protected PresentArgs Args => _presentArgs;

		/// <summary>
		/// Gets a value indicating whether the controller is disposed.
		/// </summary>
		protected bool IsDisposed => _disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="AppViewController"/> class.
		/// </summary>
		/// <param name="context">Context data for the controller instance.</param>
		protected AppViewController(IPresentableContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_id = Utility.GetNextId("_controller", ref _idCounter);
			_resourceId = Utility.GetPresentableResourceId(GetType());
			_state = context.ParentState;
			_presentArgs = context.PresentArgs;
			_view = context.ViewManager.CreateView(_resourceId, _state.Prev?.View, _presentArgs.Options);
		}

		/// <summary>
		/// Releases resources used by the controller.
		/// </summary>
		/// <param name="disposing">Should be <see langword="true"/> if the method is called from <see cref="Dispose()"/>; <see langword="false"/> otherwise.</param>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="ThrowIfDisposed"/>
		protected virtual void Dispose(bool disposing)
		{
			_disposed = true;

			if (disposing)
			{
				_view.Dispose();
			}
		}

		/// <summary>
		/// Throws an <see cref="ObjectDisposedException"/> if the controller is disposed.
		/// </summary>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="Dispose(bool)"/>
		protected void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(_id);
			}
		}

		#endregion

		#region IPresentable

		/// <inheritdoc/>
		public string Id => _id;

		/// <inheritdoc/>
		public IAppView View => _view;

		/// <inheritdoc/>
		public bool IsActive => _active;

		#endregion

		#region IPresentableEvents

		/// <summary>
		/// Called when the controller view is loaded (before transition animation). Default implementation does nothing.
		/// </summary>
		public virtual void OnViewLoaded()
		{
		}

		/// <summary>
		/// Called right after the controller transition animation finishes. Default implementation does nothing.
		/// </summary>
		public virtual void OnPresent()
		{
		}

		/// <summary>
		/// Called right before the controller becomes active. Default implementation does nothing.
		/// </summary>
		public virtual void OnActivate()
		{
			_active = true;
		}

		/// <summary>
		/// Called when the controller is about to become inactive. Default implementation does nothing.
		/// </summary>
		public virtual void OnDeactivate()
		{
			_active = false;
		}

		/// <summary>
		/// Called when the controller is about to be dismissed (before transition animation). Default implementation does nothing.
		/// </summary>
		public virtual void OnDismiss()
		{
		}

		#endregion

		#region IPresenter

		/// <inheritdoc/>
		public IAsyncOperation<IPresentable> PresentAsync(Type controllerType)
		{
			ThrowIfDisposed();
			return _context.PresentAsync(controllerType, PresentArgs.Default);
		}

		/// <inheritdoc/>
		public IAsyncOperation<IPresentable> PresentAsync(Type controllerType, PresentArgs args)
		{
			ThrowIfDisposed();
			return _context.PresentAsync(controllerType, args);
		}

		/// <inheritdoc/>
		public IAsyncOperation<TController> PresentAsync<TController>() where TController : class, IPresentable
		{
			ThrowIfDisposed();
			return _context.PresentAsync<TController>(PresentArgs.Default);
		}

		/// <inheritdoc/>
		public IAsyncOperation<TController> PresentAsync<TController>(PresentArgs args) where TController : class, IPresentable
		{
			ThrowIfDisposed();
			return _context.PresentAsync<TController>(args);
		}

		#endregion

		#region IDismissable

		/// <inheritdoc/>
		public IAsyncOperation DismissAsync()
		{
			if (_dismissOp == null)
			{
				if (_disposed)
				{
					_dismissOp = AsyncResult.CompletedOperation;
				}
				else
				{
					_dismissOp = _context.DismissAsync();
				}
			}

			return _dismissOp;
		}

		#endregion

		#region IDisposable

		/// <summary>
		/// Releases resources used by the controller.
		/// </summary>
		/// <remarks>
		/// This method is called by parent state. It should not be called by user code.
		/// </remarks>
		/// <seealso cref="Dispose(bool)"/>
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
