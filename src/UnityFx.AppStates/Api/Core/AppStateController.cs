// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// tt
	/// </summary>
	public class AppStateController : IDisposable
	{
		#region data

		private readonly AppState _state;

		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Gets a view instance attached to the controller.
		/// </summary>
		protected AppStateView View => _state.View;

		/// <summary>
		/// Gets the parent state.
		/// </summary>
		protected AppState State => _state;

		/// <summary>
		/// Gets the state creation arguments.
		/// </summary>
		protected PushStateArgs CreationArgs => _state.CreationArgs;

		/// <summary>
		/// Gets a value indicating whether the state is active.
		/// </summary>
		protected bool IsActive => _state.IsActive;

		/// <summary>
		/// Initializes a new instance of the <see cref="AppStateController"/> class.
		/// </summary>
		protected AppStateController(AppState state)
		{
			_state = state;
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
		protected IAsyncOperation<AppState> PushStateAsync<TController>(PushStateArgs args) where TController : AppStateController
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
		protected IAsyncOperation<AppState> PushStateAsync<TController>(PushOptions options, PushStateArgs args) where TController : AppStateController
		{
			ThrowIfDisposed();
			return _state.StateManager.PushStateAsync(typeof(TController), options, args);
		}

		/// <summary>
		/// Initiates the parent state exit process.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown if either the controller or its parent state is disposed.</exception>
		protected void Dismiss()
		{
			_state.DismissAsync();
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

		#endregion

		#region virtuals

		/// <summary>
		/// Called when the view is loaded (before transition animation). Default implementation does nothing.
		/// </summary>
		protected internal virtual void OnViewLoaded()
		{
		}

		/// <summary>
		/// Called right before the state becomes active. Default implementation does nothing.
		/// </summary>
		protected internal virtual void OnActivate()
		{
		}

		/// <summary>
		/// Called when the state is about to become inactive. Default implementation does nothing.
		/// </summary>
		protected internal virtual void OnDeactivate()
		{
		}

		/// <summary>
		/// Called when the state is about to be dismissed (before transition animation). Default implementation does nothing.
		/// </summary>
		protected internal virtual void OnDismiss()
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
			_disposed = true;
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
