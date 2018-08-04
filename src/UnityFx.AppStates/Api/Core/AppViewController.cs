// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
		private readonly string _typeId;
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
			_typeId = GetId(GetType());
			_state = context.ParentState;
			_presentArgs = context.PresentArgs;
			_view = context.CreateView(this);
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
				throw new ObjectDisposedException(_typeId);
			}
		}

		#endregion

		#region internals

		internal void InvokeOnViewLoaded()
		{
			ThrowIfDisposed();
			OnViewLoaded();
		}

		internal void InvokeOnPresent()
		{
			ThrowIfDisposed();
			OnPresent();
		}

		internal void InvokeOnDismiss()
		{
			ThrowIfDisposed();

			if (_childControllers != null)
			{
				foreach (var controller in _childControllers)
				{
					controller.InvokeOnDismiss();
				}
			}

			OnDismiss();
		}

		internal bool TryActivate()
		{
			ThrowIfDisposed();

			if (!_active)
			{
				_active = true;
				_view.Enabled = true;

				OnActivate();

				if (_childControllers != null)
				{
					foreach (var controller in _childControllers)
					{
						controller.TryActivate();
					}
				}

				return true;
			}

			return false;
		}

		internal bool TryDeactivate()
		{
			ThrowIfDisposed();

			if (_active)
			{
				if (_childControllers != null)
				{
					foreach (var controller in _childControllers)
					{
						controller.TryDeactivate();
					}
				}

				OnDeactivate();

				_view.Enabled = false;
				_active = false;

				return true;
			}

			return false;
		}

		internal void AddChildController(AppViewController controller)
		{
			Debug.Assert(controller != null);
			_childControllers.Add(controller);
		}

		internal AppView GetTopView()
		{
			if (_childControllers != null && _childControllers.Count > 0)
			{
				return _childControllers[_childControllers.Count - 1].GetTopView();
			}

			return _view;
		}

		internal static string GetId(Type controllerType)
		{
			if (Attribute.GetCustomAttribute(controllerType, typeof(AppViewControllerAttribute)) is AppViewControllerAttribute attr)
			{
				if (string.IsNullOrEmpty(attr.Id))
				{
					return GetIdSimple(controllerType);
				}
				else
				{
					return attr.Id;
				}
			}

			return GetIdSimple(controllerType);
		}

		internal static string GetIdSimple(Type controllerType)
		{
			var name = controllerType.Name;

			if (name.EndsWith("Controller"))
			{
				name = name.Substring(0, name.Length - 10).ToLowerInvariant();
			}

			return name;
		}

		internal static AppViewControllerOptions GetOptions(Type controllerType)
		{
			if (Attribute.GetCustomAttribute(controllerType, typeof(AppViewControllerAttribute)) is AppViewControllerAttribute attr)
			{
				return attr.Options;
			}

			return AppViewControllerOptions.None;
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
		}

		/// <summary>
		/// Called when the controller is about to become inactive. Default implementation does nothing.
		/// </summary>
		public virtual void OnDeactivate()
		{
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
		public IAsyncOperation<IPresentable> PresentAsync(Type controllerType, PresentArgs args)
		{
			ThrowIfDisposed();

			return _context.PresentAsync(controllerType, args);
		}

		/// <inheritdoc/>
		public IAsyncOperation<IPresentable> PresentAsync(Type controllerType)
		{
			ThrowIfDisposed();

			return _context.PresentAsync(controllerType, PresentArgs.Default);
		}

		/// <inheritdoc/>
		public IAsyncOperation<TController> PresentAsync<TController>(PresentArgs args) where TController : IPresentable
		{
			return PresentAsync(typeof(TController), args) as IAsyncOperation<TController>;
		}

		/// <inheritdoc/>
		public IAsyncOperation<TController> PresentAsync<TController>() where TController : IPresentable
		{
			return PresentAsync(typeof(TController)) as IAsyncOperation<TController>;
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
