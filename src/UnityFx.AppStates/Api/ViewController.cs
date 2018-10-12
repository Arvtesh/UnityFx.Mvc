// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using UnityFx.Async;

namespace UnityFx.AppStates
{
	/// <summary>
	/// A generic view controller. It is recommended to use this class as base for all other controllers.
	/// Note that minimal controller implementation should implement <see cref="IViewController"/>.
	/// </summary>
	/// <seealso cref="ViewController{TView}"/>
	public abstract class ViewController : IViewController, IPresentable, IPresentableEvents, IPresenter, ISynchronizeInvoke, IDisposable
	{
		#region data

		private readonly IViewControllerContext _context;
		private readonly string _name;

		private IAsyncOperation _dismissOp;
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Gets a value indicating whether the controller is active (i.e. can accept input).
		/// </summary>
		protected bool IsActive => _context.ParentState.IsActive;

		/// <summary>
		/// Gets the parent state.
		/// </summary>
		protected IAppState ParentState => _context.ParentState;

		/// <summary>
		/// Gets the controller creation arguments.
		/// </summary>
		protected PresentArgs Args => _context.PresentArgs;

		/// <summary>
		/// Gets a value indicating whether the controller is disposed.
		/// </summary>
		protected bool IsDisposed => _disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewController"/> class.
		/// </summary>
		/// <param name="context">Context data for the controller instance.</param>
		protected ViewController(IViewControllerContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_name = Utility.GetControllerTypeId(GetType());
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
				throw new ObjectDisposedException(Name ?? GetType().Name);
			}
		}

		/// <summary>
		/// Performs any asynchronous actions needed to present this object. Default implementation does nothing.
		/// </summary>
		/// <param name="presentContext">Context data provided by the system.</param>
		/// <returns>Returns an object that can be used to track the operation state.</returns>
		/// <seealso cref="DismissAsync(IDismissContext)"/>
		protected virtual IAsyncOperation PresentAsync(IPresentContext presentContext)
		{
			return AsyncResult.CompletedOperation;
		}

		/// <summary>
		/// Performs any asynchronous actions needed to dismiss this object. The method is invoked by the system.
		/// </summary>
		/// <param name="dismissContext">Context data provided by the system.</param>
		/// <returns>Returns an object that can be used to track the operation state.</returns>
		/// <seealso cref="PresentAsync(IPresentContext)"/>
		protected virtual IAsyncOperation DismissAsync(IDismissContext dismissContext)
		{
			return AsyncResult.CompletedOperation;
		}

		/// <summary>
		/// Called right after the controller transition animation finishes. Default implementation does nothing.
		/// </summary>
		/// <seealso cref="OnDismiss"/>
		protected virtual void OnPresent()
		{
		}

		/// <summary>
		/// Called right before the controller becomes active. Default implementation does nothing.
		/// </summary>
		/// <seealso cref="OnDeactivate"/>
		protected virtual void OnActivate()
		{
		}

		/// <summary>
		/// Called when the controller is about to become inactive. Default implementation does nothing.
		/// </summary>
		/// <seealso cref="OnActivate"/>
		protected virtual void OnDeactivate()
		{
		}

		/// <summary>
		/// Called when the controller is about to be dismissed (before transition animation). Default implementation does nothing.
		/// </summary>
		/// <seealso cref="OnPresent"/>
		protected virtual void OnDismiss()
		{
		}

		/// <summary>
		/// Called when the controller is disposed. Default implementation does nothing.
		/// </summary>
		/// <seealso cref="OnPresent"/>
		protected virtual void OnDispose()
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
			if (!_disposed)
			{
				_disposed = true;

				if (disposing)
				{
					OnDispose();
				}
			}
		}

		#endregion

		#region IViewController

		/// <summary>
		/// Gets the controller name.
		/// </summary>
		public string Name => _name;

		#endregion

		#region IPresentable

		/// <inheritdoc/>
		IAsyncOperation IPresentable.PresentAsync(IPresentContext presentContext)
		{
			Debug.Assert(presentContext != null);
			Debug.Assert(!_disposed);
			return PresentAsync(presentContext);
		}

		/// <inheritdoc/>
		IAsyncOperation IPresentable.DismissAsync(IDismissContext dismissContext)
		{
			Debug.Assert(dismissContext != null);
			Debug.Assert(!_disposed);
			return DismissAsync(dismissContext);
		}

		#endregion

		#region IPresentableEvents

		/// <inheritdoc/>
		void IPresentableEvents.OnPresent()
		{
			Debug.Assert(!_disposed);
			OnPresent();
		}

		/// <inheritdoc/>
		void IPresentableEvents.OnActivate()
		{
			Debug.Assert(!_disposed);
			OnActivate();
		}

		/// <inheritdoc/>
		void IPresentableEvents.OnDeactivate()
		{
			Debug.Assert(!_disposed);
			OnDeactivate();
		}

		/// <inheritdoc/>
		void IPresentableEvents.OnDismiss()
		{
			Debug.Assert(!_disposed);
			OnDismiss();
		}

		#endregion

		#region IPresenter

		/// <inheritdoc/>
		public IAsyncOperation<IViewController> PresentAsync(Type controllerType)
		{
			ThrowIfDisposed();
			return _context.PresentAsync(controllerType, PresentArgs.Default);
		}

		/// <inheritdoc/>
		public IAsyncOperation<IViewController> PresentAsync(Type controllerType, PresentArgs args)
		{
			ThrowIfDisposed();
			return _context.PresentAsync(controllerType, args);
		}

		/// <inheritdoc/>
		public IAsyncOperation<TController> PresentAsync<TController>() where TController : class, IViewController
		{
			ThrowIfDisposed();
			return _context.PresentAsync<TController>(PresentArgs.Default);
		}

		/// <inheritdoc/>
		public IAsyncOperation<TController> PresentAsync<TController>(PresentArgs args) where TController : class, IViewController
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

		#region ISynchronizeInvoke

		/// <summary>
		/// Gets a value indicating whether the caller must call <see cref="Invoke(Delegate, object[])"/> when calling the controller.
		/// </summary>
		/// <seealso cref="BeginInvoke(Delegate, object[])"/>
		/// <seealso cref="EndInvoke(IAsyncResult)"/>
		/// <seealso cref="Invoke(Delegate, object[])"/>
		public bool InvokeRequired => _context.InvokeRequired;

		/// <summary>
		/// Asynchronously executes the delegate on the thread that created the controller.
		/// </summary>
		/// <param name="method">A delegate to a method that takes parameters of the same number and type that are contained in <paramref name="args"/>.</param>
		/// <param name="args">An array of type <see cref="object"/> to pass as arguments to the given <paramref name="method"/>. This can be <see langword="null"/> if no arguments are needed.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="method"/> is <see langword="null"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the controller is disposed.</exception>
		/// <returns>An <see cref="IAsyncResult"/> interface that represents the asynchronous operation started by calling the <paramref name="method"/>.</returns>
		/// <seealso cref="EndInvoke(IAsyncResult)"/>
		/// <seealso cref="Invoke(Delegate, object[])"/>
		/// <seealso cref="InvokeRequired"/>
		public IAsyncResult BeginInvoke(Delegate method, object[] args)
		{
			ThrowIfDisposed();
			return _context.BeginInvoke(method, args);
		}

		/// <summary>
		/// Waits until the process started by calling <see cref="BeginInvoke(Delegate, object[])"/> completes, and then returns the value generated by the process.
		/// </summary>
		/// <param name="result">An <see cref="IAsyncResult"/> interface that represents the asynchronous operation started by calling <see cref="BeginInvoke(Delegate, object[])"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="result"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if the <paramref name="result"/> is not created with <see cref="BeginInvoke(Delegate, object[])"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the controller is disposed.</exception>
		/// <returns>An <see cref="object"/> that represents the return value generated by the asynchronous operation.</returns>
		/// <seealso cref="BeginInvoke(Delegate, object[])"/>
		/// <seealso cref="Invoke(Delegate, object[])"/>
		/// <seealso cref="InvokeRequired"/>
		public object EndInvoke(IAsyncResult result)
		{
			ThrowIfDisposed();
			return _context.EndInvoke(result);
		}

		/// <summary>
		/// Synchronously executes the delegate on the thread that created the controller and marshals the call to the creating thread.
		/// </summary>
		/// <param name="method">A delegate that contains a method to call, in the context of the thread for the controller.</param>
		/// <param name="args">An array of type <see cref="object"/> that represents the arguments to pass to the given <paramref name="method"/>. This can be <see langword="null"/> if no arguments are needed.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="method"/> is <see langword="null"/>.</exception>
		/// <exception cref="ObjectDisposedException">Thrown if the controller is disposed.</exception>
		/// <returns>An <see cref="object"/> that represents the return value from the delegate being invoked, or <see langword="null"/> if the delegate has no return value.</returns>
		/// <seealso cref="BeginInvoke(Delegate, object[])"/>
		/// <seealso cref="EndInvoke(IAsyncResult)"/>
		/// <seealso cref="InvokeRequired"/>
		public object Invoke(Delegate method, object[] args)
		{
			ThrowIfDisposed();
			return _context.Invoke(method, args);
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
