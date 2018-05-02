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
		private readonly AppView _view;

		private AppViewController _parentController;
		private List<AppViewController> _childControllers;
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Gets the parent state.
		/// </summary>
		protected AppState State => _state;

		/// <summary>
		/// Gets a view instance attached to the controller.
		/// </summary>
		protected AppView View => _view;

		/// <summary>
		/// Gets the state creation arguments.
		/// </summary>
		protected PushStateArgs CreationArgs => _state.CreationArgs;

		/// <summary>
		/// Gets a value indicating whether the state is active.
		/// </summary>
		protected bool IsActive => _state.IsActive;

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
			_view = state.View;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppViewController"/> class.
		/// </summary>
		protected AppViewController(AppState state, AppView view)
		{
			_state = state ?? throw new ArgumentNullException(nameof(state));
			_view = view ?? throw new ArgumentNullException(nameof(view));
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
				throw new ObjectDisposedException(GetType().Name);
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

			return AddChildControllerInternal(controllerType);
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
			return AddChildControllerInternal(typeof(TController)) as TController;
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
		/// Pushes a new state with the specified controller onto the stack.
		/// </summary>
		/// <param name="controllerType">Type of the state controller.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controllerType"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="controllerType"/> cannot be used to instantiate state controller (for instance it is abstract type).</exception>
		/// <exception cref="InvalidOperationException">Too many operations are scheduled already.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if either the controller or its parent state is disposed.</exception>
		protected IAsyncOperation<AppState> PushStateAsync(Type controllerType, PushStateArgs args)
		{
			ThrowIfDisposed();
			return _state.StateManager.PushStateAsync(controllerType, args);
		}

		/// <summary>
		/// Pushes a new state with the specified controller onto the stack.
		/// </summary>
		/// <param name="controllerType">Type of the state controller.</param>
		/// <param name="options">Push options.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="controllerType"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="controllerType"/> cannot be used to instantiate state controller (for instance it is abstract type).</exception>
		/// <exception cref="InvalidOperationException">Too many operations are scheduled already.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if either the controller or its parent state is disposed.</exception>
		protected IAsyncOperation<AppState> PushStateAsync(Type controllerType, PushOptions options, PushStateArgs args)
		{
			ThrowIfDisposed();
			return _state.StateManager.PushStateAsync(controllerType, options, args);
		}

		/// <summary>
		/// Pushes a new state with the specified controller onto the stack.
		/// </summary>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="InvalidOperationException">Too many operations are scheduled already.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if either the controller or its parent state is disposed.</exception>
		protected IAsyncOperation<AppState> PushStateAsync<TController>(PushStateArgs args) where TController : AppViewController
		{
			ThrowIfDisposed();
			return _state.StateManager.PushStateAsync(typeof(TController), args);
		}

		/// <summary>
		/// Pushes a new state with the specified controller onto the stack.
		/// </summary>
		/// <param name="options">Push options.</param>
		/// <param name="args">Controller arguments.</param>
		/// <returns>An object that can be used to track the operation progress.</returns>
		/// <exception cref="InvalidOperationException">Too many operations are scheduled already.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if either the controller or its parent state is disposed.</exception>
		protected IAsyncOperation<AppState> PushStateAsync<TController>(PushOptions options, PushStateArgs args) where TController : AppViewController
		{
			ThrowIfDisposed();
			return _state.StateManager.PushStateAsync(typeof(TController), options, args);
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

			if (_childControllers != null)
			{
				foreach (var controller in _childControllers)
				{
					controller.OnViewLoaded();
				}
			}
		}

		internal void InvokeOnActivate()
		{
			OnActivate();

			if (_childControllers != null)
			{
				foreach (var controller in _childControllers)
				{
					controller.OnActivate();
				}
			}
		}

		internal void InvokeOnDeactivate()
		{
			if (_childControllers != null)
			{
				foreach (var controller in _childControllers)
				{
					controller.OnDeactivate();
				}
			}

			OnDeactivate();
		}

		internal void InvokeOnDismiss()
		{
			if (_childControllers != null)
			{
				foreach (var controller in _childControllers)
				{
					controller.OnDismiss();
				}
			}

			OnDismiss();
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
			if (_parentController != null)
			{
				_parentController.RemoveChildController(this);
				_parentController = null;
			}

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

		private AppViewController AddChildControllerInternal(Type controllerType)
		{
			ThrowIfDisposed();

			if (_childControllers == null)
			{
				_childControllers = new List<AppViewController>();
			}

			var c = _state.StateManager.Shared.ControllerFactory.CreateController(controllerType, _state);

			if (!c.IsDisposed)
			{
				c._parentController = this;
				_childControllers.Add(c);
			}

			return c;
		}

		#endregion
	}
}
