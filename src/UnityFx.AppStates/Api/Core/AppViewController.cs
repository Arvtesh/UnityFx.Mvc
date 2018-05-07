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
	public class AppViewController : IDisposable
	{
		#region data

		private readonly AppState _state;
		private readonly AppViewController _parentController;
		private readonly AppView _view;
		private readonly string _id;
		private readonly AppViewControllerOptions _options;
		private readonly object _args;

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
		protected AppViewControllerOptions CreationOptions => _options;

		/// <summary>
		/// Gets the parent state.
		/// </summary>
		protected AppState State => _state;

		/// <summary>
		/// Gets the controller creation arguments.
		/// </summary>
		protected object Args => _args;

		/// <summary>
		/// Gets a value indicating whether the controller is disposed.
		/// </summary>
		protected bool IsDisposed => _disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="AppViewController"/> class.
		/// </summary>
		protected AppViewController(AppState state)
		{
			_state = state ?? throw new ArgumentNullException(nameof(state));
			_id = GetId(GetType());
			_options = GetOptions(GetType());
			_parentController = state.TmpController;
			_args = state.TmpControllerArgs;

			if (_parentController == null)
			{
				_view = state.ViewManager.CreateView(_id, state.GetPrevView(), AppViewOptions.None);
			}
			else if ((_options & AppViewControllerOptions.ReuseParentView) != 0)
			{
				_view = state.ViewManager.CreateChildView(_id, _parentController.View, AppViewOptions.None);
			}
			else
			{
				_view = state.ViewManager.CreateView(_id, _parentController.GetTopView(), AppViewOptions.None);
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

		#region child controllers

		/// <summary>
		/// Gets child view controllers.
		/// </summary>
		protected IEnumerable<AppViewController> ChildControllers => _childControllers ?? Enumerable.Empty<AppViewController>();

		/// <summary>
		/// Adds a new child controller. The controller created is attached to the same state and view as the caller controller.
		/// </summary>
		/// <param name="controllerType">Type of the controller to instantiate.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controllerType"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="controllerType"/> cannot be used to instantiate a controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the controller is disposed.</exception>
		/// <returns>Returns the created controller instance.</returns>
		protected AppViewController AddChildController(Type controllerType)
		{
			if (controllerType == null)
			{
				throw new ArgumentNullException(nameof(controllerType));
			}

			return AddChildControllerInternal(controllerType, PresentOptions.None, null);
		}

		/// <summary>
		/// Adds a new child controller. The controller created is attached to the same state and view as the caller controller.
		/// </summary>
		/// <typeparam name="TController">Type of the controller to instantiate.</typeparam>
		/// <exception cref="ArgumentException">Thrown if <typeparamref name="TController"/> cannot be used to instantiate a controller (for instance it is abstract type).</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the controller is disposed.</exception>
		/// <returns>Returns the created controller instance.</returns>
		protected TController AddChildController<TController>() where TController : AppViewController
		{
			return AddChildControllerInternal(typeof(TController), PresentOptions.None, null) as TController;
		}

		/// <summary>
		/// Removes the specified controller from the list of child controllers. Does not dispose the controller.
		/// </summary>
		/// <param name="controller">The controller instance to remove. This value can be <see langword="null"/>.</param>
		/// <returns>Returns <see langword="true"/> if controller is successfully removed; otherwise, <see langword="false"/>.</returns>
		protected bool RemoveChildController(AppViewController controller)
		{
			if (controller != null && _childControllers != null)
			{
				return _childControllers.Remove(controller);
			}

			return false;
		}

		#endregion

		#region state management

		/// <summary>
		/// Presents a new state with the specified controller as a child state.
		/// </summary>
		/// <param name="controllerType">Type of the view controller to present.</param>
		/// <param name="options">Presentation options.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controllerType"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="controllerType"/> cannot be used to instantiate state controller (for instance it is abstract type).</exception>
		/// <exception cref="InvalidOperationException">Too many operations are scheduled already.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if either the controller or its parent state is disposed.</exception>
		protected IAsyncOperation<AppState> PresentAsync(Type controllerType, PresentOptions options, PresentArgs args)
		{
			ThrowIfDisposed();
			throw new NotImplementedException();
		}

		/// <summary>
		/// Pushes a new state with the specified controller onto the stack.
		/// </summary>
		/// <param name="controllerType">Type of the state controller.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controllerType"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="controllerType"/> cannot be used to instantiate state controller (for instance it is abstract type).</exception>
		/// <exception cref="InvalidOperationException">Too many operations are scheduled already.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if either the controller or its parent state is disposed.</exception>
		protected IAsyncOperation<AppState> PresentAsync(Type controllerType, PresentArgs args)
		{
			ThrowIfDisposed();
			throw new NotImplementedException();
		}

		/// <summary>
		/// Pushes a new state with the specified controller onto the stack.
		/// </summary>
		/// <param name="options">Push options.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="InvalidOperationException">Too many operations are scheduled already.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if either the controller or its parent state is disposed.</exception>
		protected IAsyncOperation<AppState> PresentAsync<TController>(PresentOptions options, PresentArgs args) where TController : AppViewController
		{
			ThrowIfDisposed();
			throw new NotImplementedException();
		}

		/// <summary>
		/// Pushes a new state with the specified controller onto the stack.
		/// </summary>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="InvalidOperationException">Too many operations are scheduled already.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if either the controller or its parent state is disposed.</exception>
		protected IAsyncOperation<AppState> PresentAsync<TController>(PresentArgs args) where TController : AppViewController
		{
			ThrowIfDisposed();
			throw new NotImplementedException();
		}

		/// <summary>
		/// Initiates the parent state dismissal.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown if either the controller or its parent state is disposed.</exception>
		protected void Dismiss()
		{
			_state.DismissAsync();
		}

		#endregion

		#region internals

		internal void InvokeOnViewLoaded()
		{
			OnViewLoaded();
		}

		internal void InvokeOnActivate()
		{
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
			if (_childControllers != null)
			{
				foreach (var controller in _childControllers)
				{
					controller.InvokeOnDismiss();
				}
			}

			OnDismiss();
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
		/// Called when the view is loaded (before transition animation). Default implementation does nothing.
		/// </summary>
		protected virtual void OnViewLoaded()
		{
		}

		/// <summary>
		/// Called right before the state becomes active. Default implementation does nothing.
		/// </summary>
		protected virtual void OnActivate()
		{
		}

		/// <summary>
		/// Called when the state is about to become inactive. Default implementation does nothing.
		/// </summary>
		protected virtual void OnDeactivate()
		{
		}

		/// <summary>
		/// Called when the state is about to be dismissed (before transition animation). Default implementation does nothing.
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
			_disposed = true;
			Dispose(true);
			DisposeInternal();
			GC.SuppressFinalize(this);
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

		private AppView GetTopView()
		{
			if (_childControllers != null && _childControllers.Count > 0)
			{
				return _childControllers[_childControllers.Count - 1].GetTopView();
			}

			return _view;
		}

		private AppViewController AddChildControllerInternal(Type controllerType, PresentOptions options, object args)
		{
			ThrowIfDisposed();

			if (_childControllers == null)
			{
				_childControllers = new List<AppViewController>();
			}

			_state.TmpController = this;
			_state.TmpControllerOptions = options;
			_state.TmpControllerArgs = args;

			try
			{
				var controller = _state.ControllerFactory.CreateController(controllerType, _state);

				if (!controller.IsDisposed)
				{
					_childControllers.Add(controller);

					if (controller.View.IsLoaded)
					{
						OnViewLoaded(AsyncResult.CompletedOperation);
					}
					else
					{
						controller.View.Load().AddCompletionCallback(OnViewLoaded);
					}
				}

				return controller;
			}
			finally
			{
				_state.TmpControllerArgs = null;
				_state.TmpControllerOptions = PresentOptions.None;
				_state.TmpController = null;
			}
		}

		private void OnViewLoaded(IAsyncOperation op)
		{
			InvokeOnViewLoaded();

			if (_state.IsActive)
			{
				InvokeOnActivate();
			}
		}

		#endregion
	}
}
