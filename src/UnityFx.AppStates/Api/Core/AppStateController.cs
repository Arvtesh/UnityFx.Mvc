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
	public class AppStateController : IAppStateController, IDisposable
	{
		#region data

		private readonly AppState _state;

		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Gets a view instance attached to the state.
		/// </summary>
		protected IAppStateView View => _state.View;

		/// <summary>
		/// Gets the parent state.
		/// </summary>
		protected AppState State => _state;

		/// <summary>
		/// Gets the parent state manager.
		/// </summary>
		protected IAppStateManager StateManager => _state.StateManager;

		/// <summary>
		/// Gets a manager for substates.
		/// </summary>
		protected IAppStateManager SubstateManager => _state.SubstateManager;

		/// <summary>
		/// Initializes a new instance of the <see cref="AppStateController"/> class.
		/// </summary>
		protected AppStateController(AppState state)
		{
			_state = state;
		}

		/// <summary>
		/// Exits the state.
		/// </summary>
		/// <exception cref="ObjectDisposedException">Thrown if the controller instance is disposed.</exception>
		protected void Close()
		{
			ThrowIfDisposed();
			_state.StateManager.PopStateAsync(_state);
		}

		/// <summary>
		/// Throws an <see cref="ObjectDisposedException"/> if the instance is already disposed.
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

		/// <summary>
		/// Called before the first activation to load state content. Default implementation does nothing.
		/// </summary>
		protected internal virtual IAsyncOperation OnLoadContent()
		{
			return AsyncResult.CompletedOperation;
		}

		/// <summary>
		/// Called right before the state becomes active. Default implementation does nothing.
		/// </summary>
		protected internal virtual void OnActivate(bool firstActivated)
		{
		}

		/// <summary>
		/// Called when the state is about to become inactive. Default implementation does nothing.
		/// </summary>
		protected internal virtual void OnDeactivate()
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

		/// <inheritdoc/>
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
