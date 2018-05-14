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
	/// tt
	/// </summary>
	public class AppViewController : IPresenter, IDismissable
	{
		#region data

		private readonly AppState _state;
		private readonly AppViewController _parentController;
		private readonly AppView _view;
		private readonly string _id;
		private readonly AppViewControllerOptions _createOptions;
		private readonly PresentOptions _presentOptions;
		private readonly PresentArgs _presentArgs;

		private List<AppViewController> _childControllers;
		private bool _active;
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Gets the controller identifier.
		/// </summary>
		public string Id => _id;

		/// <summary>
		/// Gets the parent state.
		/// </summary>
		public AppState State => _state;

		/// <summary>
		/// Gets a view instance attached to the controller.
		/// </summary>
		public AppView View => _view;

		/// <summary>
		/// Gets a value indicating whether the state is active.
		/// </summary>
		public bool IsActive => _active;

		/// <summary>
		/// Gets creation options for the controller.
		/// </summary>
		protected AppViewControllerOptions CreationOptions => _createOptions;

		/// <summary>
		/// Gets the controller creation options.
		/// </summary>
		protected PresentOptions PresentOptions => _presentOptions;

		/// <summary>
		/// Gets the controller creation arguments.
		/// </summary>
		protected PresentArgs Args => _presentArgs;

		/// <summary>
		/// Gets a value indicating whether the controller is disposed.
		/// </summary>
		protected bool IsDisposed => _disposed;

		/// <summary>
		/// Gets child view controllers.
		/// </summary>
		protected IEnumerable<AppViewController> ChildControllers => _childControllers ?? Enumerable.Empty<AppViewController>();

		/// <summary>
		/// Initializes a new instance of the <see cref="AppViewController"/> class.
		/// </summary>
		protected AppViewController(AppState state)
		{
			try
			{
				_state = state ?? throw new ArgumentNullException(nameof(state));

				_id = GetId(GetType());
				_createOptions = GetOptions(GetType());
				_parentController = state.TmpController;
				_presentOptions = state.TmpControllerOptions;
				_presentArgs = state.TmpControllerArgs;

				if (_parentController == null)
				{
					_view = state.ViewManager.CreateView(_id, state.GetPrevView(), AppViewOptions.None);
				}
				else if ((_createOptions & AppViewControllerOptions.ReuseParentView) != 0)
				{
					_view = state.ViewManager.CreateChildView(_id, _parentController.View, AppViewOptions.None);
				}
				else
				{
					_view = state.ViewManager.CreateView(_id, _parentController.GetTopView(), AppViewOptions.None);
				}
			}
			finally
			{
				state.TmpControllerArgs = null;
				state.TmpControllerOptions = PresentOptions.None;
				state.TmpController = null;
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

		internal void InvokeOnActivate()
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
						controller.InvokeOnActivate();
					}
				}
			}
		}

		internal void InvokeOnDeactivate()
		{
			ThrowIfDisposed();

			if (_active)
			{
				if (_childControllers != null)
				{
					foreach (var controller in _childControllers)
					{
						controller.InvokeOnDeactivate();
					}
				}

				OnDeactivate();

				_view.Enabled = false;
				_active = false;
			}
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

		internal AppViewController CreateChildController(Type controllerType, PresentOptions options, PresentArgs args)
		{
			ThrowIfDisposed();

			_state.TmpControllerArgs = args;
			_state.TmpControllerOptions = options;
			_state.TmpController = this;

			try
			{
				var controller = _state.ControllerFactory.CreateController(controllerType, _state);
				_childControllers.Add(controller);
				return controller;
			}
			finally
			{
				_state.TmpControllerArgs = null;
				_state.TmpControllerOptions = PresentOptions.None;
				_state.TmpController = null;
			}
		}

		internal AppView GetTopView()
		{
			if (_childControllers != null && _childControllers.Count > 0)
			{
				return _childControllers[_childControllers.Count - 1].GetTopView();
			}

			return _view;
		}

		internal static void ValidateControllerType(Type controllerType)
		{
			if (controllerType == null)
			{
				throw new ArgumentNullException(nameof(controllerType));
			}

			if (controllerType.IsAbstract)
			{
				throw new ArgumentException($"Cannot instantiate abstract type {controllerType.Name}", nameof(controllerType));
			}

			if (!controllerType.IsSubclassOf(typeof(AppViewController)))
			{
				throw new ArgumentException($"A state controller is expected to inherit " + typeof(AppViewController).Name, nameof(controllerType));
			}
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

		#region virtuals

		/// <summary>
		/// Called when the controller view is loaded (before transition animation). Default implementation does nothing.
		/// </summary>
		protected virtual void OnViewLoaded()
		{
		}

		/// <summary>
		/// Called right after the controller transition animation finishes. Default implementation does nothing.
		/// </summary>
		protected virtual void OnPresent()
		{
		}

		/// <summary>
		/// Called right before the controller becomes active. Default implementation does nothing.
		/// </summary>
		protected virtual void OnActivate()
		{
		}

		/// <summary>
		/// Called when the controller is about to become inactive. Default implementation does nothing.
		/// </summary>
		protected virtual void OnDeactivate()
		{
		}

		/// <summary>
		/// Called when the controller is about to be dismissed (before transition animation). Default implementation does nothing.
		/// </summary>
		protected virtual void OnDismiss()
		{
		}

		/// <summary>
		/// Releases resources used by the controller.
		/// </summary>
		/// <param name="disposing">Should be <see langword="true"/> if the method is called from <see cref="Dispose()"/>; <see langword="false"/> otherwise.</param>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="ThrowIfDisposed"/>
		protected virtual void Dispose(bool disposing)
		{
		}

		#endregion

		#region IPresenter

		/// <inheritdoc/>
		public IAsyncOperation<AppViewController> PresentAsync(Type controllerType, PresentOptions options, PresentArgs args)
		{
			ThrowIfDisposed();

			if ((options & PresentOptions.Child) != 0)
			{
				if (_childControllers == null)
				{
					_childControllers = new List<AppViewController>();
				}

				return _state.PresentAsync(this, controllerType, options, args);
			}
			else
			{
				return _state.PresentAsync(controllerType, options, args);
			}
		}

		/// <inheritdoc/>
		public IAsyncOperation<AppViewController> PresentAsync(Type controllerType, PresentArgs args)
		{
			ThrowIfDisposed();

			return _state.PresentAsync(controllerType, args);
		}

		/// <inheritdoc/>
		public IAsyncOperation<TController> PresentAsync<TController>(PresentOptions options, PresentArgs args) where TController : AppViewController
		{
			return PresentAsync(typeof(TController), options, args) as IAsyncOperation<TController>;
		}

		/// <inheritdoc/>
		public IAsyncOperation<TController> PresentAsync<TController>(PresentArgs args) where TController : AppViewController
		{
			return PresentAsync(typeof(TController), args) as IAsyncOperation<TController>;
		}

		#endregion

		#region IDismissable

		/// <inheritdoc/>
		public IAsyncOperation DismissAsync()
		{
			ThrowIfDisposed();

			if (_parentController != null)
			{
				return _state.DismissAsync(this);
			}
			else
			{
				return _state.DismissAsync();
			}
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
			if (!_disposed)
			{
				_disposed = true;
				Dispose(true);
				DisposeInternal();
				GC.SuppressFinalize(this);
			}
		}

		#endregion

		#region implementation

		private void DisposeInternal()
		{
			_parentController?.RemoveChildController(this);
			_view.Dispose();

			if (_childControllers != null)
			{
				foreach (var controller in _childControllers)
				{
					controller.Dispose();
				}

				_childControllers.Clear();
				_childControllers = null;
			}
		}

		private bool RemoveChildController(AppViewController controller)
		{
			if (controller != null && _childControllers != null)
			{
				return _childControllers.Remove(controller);
			}

			return false;
		}

		#endregion
	}
}
