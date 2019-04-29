// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace UnityFx.Mvc
{
	/// <summary>
	/// Implementation of <see cref="IViewController"/>.
	/// </summary>
	/// <seealso cref="ViewController{TView}"/>
	public abstract class ViewController : IViewController
	{
		#region data

		private ViewOptions _viewOptions;
		private IView _view;
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Defines view-related flags.
		/// </summary>
		[Flags]
		public enum ViewOptions
		{
			/// <summary>
			/// No options.
			/// </summary>
			None = 0,

			/// <summary>
			/// If set, the view managed by the controller is not disposed.
			/// </summary>
			DoNotDispose = 1
		}

		/// <summary>
		/// Gets a value indicating whether the controller is disposed.
		/// </summary>
		protected bool IsDisposed => _disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewController"/> class.
		/// </summary>
		protected ViewController()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewController"/> class.
		/// </summary>
		/// <param name="view">A view managed by the controller.</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="view"/> is <see langword="null"/>.</exception>
		protected ViewController(IView view)
		{
			_view = view ?? throw new ArgumentNullException(nameof(view));
			_view.Command += OnCommand;
			_view.Disposed += OnViewDisposed;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewController"/> class.
		/// </summary>
		/// <param name="view">A view managed by the controller.</param>
		/// <param name="viewOptions">View-related flags.</param>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="view"/> is <see langword="null"/>.</exception>
		protected ViewController(IView view, ViewOptions viewOptions)
		{
			_view = view ?? throw new ArgumentNullException(nameof(view));
			_view.Command += OnCommand;
			_view.Disposed += OnViewDisposed;
			_viewOptions = viewOptions;
		}

		/// <summary>
		/// Sets the view value.
		/// </summary>
		/// <param name="view">The view reference to set.</param>
		/// <param name="viewOptions">View-related flags.</param>
		/// <exception cref="ObjectDisposedException">Thrown if the controller is disposed.</exception>
		protected void SetView(IView view, ViewOptions viewOptions)
		{
			if (_view != null)
			{
				_view.Command -= OnCommand;
				_view.Disposed -= OnViewDisposed;
			}

			_view = view;
			_viewOptions = viewOptions;

			if (_view != null)
			{
				_view.Command += OnCommand;
				_view.Disposed += OnViewDisposed;
			}
		}

		/// <summary>
		/// Throws an <see cref="ObjectDisposedException"/> if the controller is disposed.
		/// </summary>
		/// <seealso cref="Dispose()"/>
		protected void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		/// <summary>
		/// Called to process a command.
		/// </summary>
		/// <returns>Returns <see langword="true"/> if the command has been handles; <see langword="false"/> otherwise.</returns>
		protected virtual bool OnCommand(CommandEventArgs e)
		{
			return false;
		}

		/// <summary>
		/// Initiates loading view. Default implementation does nothing.
		/// </summary>
		protected virtual void OnLoadViewCompleted(AsyncCompletedEventArgs args)
		{
			if (!_disposed)
			{
				LoadViewCompleted?.Invoke(this, args);
			}
		}

		/// <summary>
		/// Initiates loading view. Default implementation does nothing.
		/// </summary>
		protected virtual void OnLoadView()
		{
		}

		/// <summary>
		/// Called when the controller is being disposed. Should not thiw exceptions. Default implementation unloads the attached view.
		/// </summary>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="ThrowIfDisposed"/>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				UnloadView();
			}
		}

		#endregion

		#region IViewController

		/// <summary>
		/// Raised when the controller has been disposed.
		/// </summary>
		/// <seealso cref="Dispose()"/>
		/// <seealso cref="Dispose(bool)"/>
		public event EventHandler Disposed;

		/// <summary>
		/// Raised when the controller <see cref="View"/> has been loaded.
		/// </summary>
		/// <seealso cref="View"/>
		/// <seealso cref="IsViewLoaded"/>
		/// <seealso cref="LoadViewAsync"/>
		public event EventHandler<AsyncCompletedEventArgs> LoadViewCompleted;

		/// <summary>
		/// Gets a value indicating whether the <see cref="View"/> can be safely used.
		/// </summary>
		/// <seealso cref="View"/>
		/// <seealso cref="LoadViewAsync"/>
		public bool IsViewLoaded => _view != null;

		/// <summary>
		/// Gets a view managed by the controller. Returns <see langword="null"/> if the view is not loaded.
		/// </summary>
		/// <seealso cref="IsViewLoaded"/>
		/// <seealso cref="LoadViewAsync"/>
		/// <seealso cref="UnloadView"/>
		public IView View => _view;

		/// <summary>
		/// Loads <see cref="View"/>. If view is already loaded (or another load operation is already running) the method returns immediately.
		/// </summary>
		/// <remarks>
		/// Implementation may decide to load views asynchronously. In this case the method just initiates the operation and returns.
		/// </remarks>
		/// <exception cref="InvalidOperationException">Thrown if unload operation is pending.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the controller is disposed.</exception>
		/// <seealso cref="View"/>
		/// <seealso cref="LoadViewCompleted"/>
		/// <seealso cref="UnloadView"/>
		public void LoadViewAsync()
		{
			ThrowIfDisposed();

			if (_view == null)
			{
				OnLoadView();
			}
		}

		/// <summary>
		/// Unloads the <see cref="View"/> (if loaded).
		/// </summary>
		/// <seealso cref="View"/>
		/// <seealso cref="LoadViewAsync"/>
		public void UnloadView()
		{
			if (_view != null)
			{
				_view.Command -= OnCommand;
				_view.Disposed -= OnViewDisposed;

				try
				{
					if ((_viewOptions & ViewOptions.DoNotDispose) == 0)
					{
						_view.Dispose();
					}
				}
				finally
				{
					_view = null;
				}
			}
		}

		#endregion

		#region ICommandTarget

		/// <summary>
		/// Invokes a command.
		/// </summary>
		/// <param name="commandName">Name of the command to invoke.</param>
		/// <param name="args">Command-specific arguments.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="commandName"/> is <see langword="null"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the controller is disposed.</exception>
		/// <returns>Returns <see langword="true"/> if the command has been handles; <see langword="false"/> otherwise.</returns>
		public bool InvokeCommand(string commandName, object args)
		{
			ThrowIfDisposed();

			if (commandName == null)
			{
				throw new ArgumentNullException(nameof(commandName));
			}

			return OnCommand(new CommandEventArgs(commandName, args));
		}

		#endregion

		#region IDisposable

		/// <summary>
		/// Releases resources used by the controller.
		/// </summary>
		/// <seealso cref="ThrowIfDisposed"/>
		/// <seealso cref="Dispose(bool)"/>
		/// <seealso cref="Disposed"/>
		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				Dispose(true);
				Disposed?.Invoke(this, EventArgs.Empty);
			}
		}

		#endregion

		#region implementation

		private void OnViewDisposed(object sender, EventArgs args)
		{
			_view = null;
		}

		private void OnCommand(object sender, CommandEventArgs e)
		{
			Debug.Assert(e != null);
			Debug.Assert(!_disposed);

			OnCommand(e);
		}

		#endregion
	}
}
